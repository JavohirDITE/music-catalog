import { useState, useEffect, useCallback, useRef } from 'react';
import { Toolbar } from './components/Toolbar';
import { TableView } from './components/TableView';
import { GalleryView } from './components/GalleryView';
import { fetchSongs } from './api/client';
import { Song } from './types/Song';
import './App.css';

function App() {
    const [locale, setLocale] = useState('en-US');
    const [seed, setSeed] = useState('12345');
    const [likes, setLikes] = useState(5.0);
    const [view, setView] = useState<'table' | 'gallery'>('table');

    const [songs, setSongs] = useState<Song[]>([]);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(20);
    const [loading, setLoading] = useState(false);
    const [hasMore, setHasMore] = useState(true);

    const debounceRef = useRef<number | null>(null);
    const galleryContainerRef = useRef<HTMLDivElement | null>(null);
    const fetchKeyRef = useRef(0);

    const loadSongs = useCallback(async (targetPage: number, append: boolean = false) => {
        setLoading(true);
        fetchKeyRef.current += 1;
        const currentFetchKey = fetchKeyRef.current;

        try {
            const data = await fetchSongs(locale, seed, likes, targetPage, pageSize);

            if (currentFetchKey !== fetchKeyRef.current) {
                return;
            }

            if (append) {
                setSongs(prev => [...prev, ...data.items]);
            } else {
                setSongs(data.items);
            }
            setHasMore(data.items.length === pageSize);
        } catch (error) {
            console.error('Failed to load songs:', error);
        } finally {
            if (currentFetchKey === fetchKeyRef.current) {
                setLoading(false);
            }
        }
    }, [locale, seed, likes, pageSize]);

    useEffect(() => {
        if (debounceRef.current) {
            clearTimeout(debounceRef.current);
        }

        debounceRef.current = setTimeout(() => {
            if (view === 'gallery' && galleryContainerRef.current) {
                galleryContainerRef.current.scrollTo(0, 0);
            }
            setPage(1);
            loadSongs(1, false);
        }, 200);

        return () => {
            if (debounceRef.current) {
                clearTimeout(debounceRef.current);
            }
        };
    }, [locale, seed, likes, pageSize, loadSongs]);

    useEffect(() => {
        loadSongs(page, view === 'gallery' && page > 1);
    }, [page, loadSongs, view]);

    const handleRandomSeed = () => {
        const max = BigInt('18446744073709551615');
        const randomBigInt = BigInt(Math.floor(Math.random() * Number.MAX_SAFE_INTEGER)) *
            BigInt(Math.floor(Math.random() * 1000000));
        const clampedSeed = randomBigInt % max;
        setSeed(clampedSeed.toString());
    };

    const handlePageChange = (newPage: number) => {
        if (newPage >= 1 && newPage !== page) {
            setPage(newPage);
        }
    };

    const handlePageSizeChange = (newPageSize: number) => {
        setPageSize(newPageSize);
        setPage(1);
    };

    const handleLoadMore = () => {
        if (!loading && hasMore) {
            setPage(prev => prev + 1);
        }
    };

    const handleViewChange = (newView: 'table' | 'gallery') => {
        if (newView !== view) {
            setView(newView);
            setPage(1);
            setSongs([]);
            setTimeout(() => {
                loadSongs(1, false);
            }, 0);
        }
    };

    return (
        <div className="app">
            <Toolbar
                locale={locale}
                seed={seed}
                likes={likes}
                view={view}
                onLocaleChange={setLocale}
                onSeedChange={setSeed}
                onLikesChange={setLikes}
                onViewChange={handleViewChange}
                onRandomSeed={handleRandomSeed}
            />
            <main ref={galleryContainerRef}>
                {view === 'table' ? (
                    <TableView
                        songs={songs}
                        page={page}
                        pageSize={pageSize}
                        onPageChange={handlePageChange}
                        onPageSizeChange={handlePageSizeChange}
                    />
                ) : (
                    <GalleryView
                        songs={songs}
                        onLoadMore={handleLoadMore}
                        hasMore={hasMore}
                        loading={loading}
                    />
                )}
            </main>
        </div>
    );
}

export default App;
