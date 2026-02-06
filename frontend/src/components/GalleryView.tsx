import React, { useEffect, useRef, useCallback, useState } from 'react';
import { Song } from '../types/Song';

interface GalleryViewProps {
    songs: Song[];
    onLoadMore: () => void;
    hasMore: boolean;
    loading: boolean;
}

export const GalleryView: React.FC<GalleryViewProps> = ({
    songs,
    onLoadMore,
    hasMore,
    loading
}) => {
    const observerRef = useRef<IntersectionObserver | null>(null);
    const loadMoreRef = useRef<HTMLDivElement | null>(null);
    const [expandedIndex, setExpandedIndex] = useState<number | null>(null);

    const handleObserver = useCallback((entries: IntersectionObserverEntry[]) => {
        const [entry] = entries;
        if (entry.isIntersecting && hasMore && !loading) {
            onLoadMore();
        }
    }, [hasMore, loading, onLoadMore]);

    useEffect(() => {
        if (observerRef.current) {
            observerRef.current.disconnect();
        }

        observerRef.current = new IntersectionObserver(handleObserver, {
            root: null,
            rootMargin: '100px',
            threshold: 0
        });

        if (loadMoreRef.current) {
            observerRef.current.observe(loadMoreRef.current);
        }

        return () => {
            if (observerRef.current) {
                observerRef.current.disconnect();
            }
        };
    }, [handleObserver]);

    const toggleExpand = (index: number) => {
        setExpandedIndex(expandedIndex === index ? null : index);
    };

    return (
        <div className="gallery-view">
            <div className="gallery-grid">
                {songs.map((song) => (
                    <div
                        key={song.index}
                        className={`gallery-card ${expandedIndex === song.index ? 'expanded' : ''}`}
                        onClick={() => toggleExpand(song.index)}
                    >
                        <img src={song.coverUrl} alt={song.title} className="gallery-cover" />
                        <div className="gallery-info">
                            <h3>{song.title}</h3>
                            <p className="artist">{song.artist}</p>
                            <p className="album">{song.album}</p>
                            <div className="meta-row">
                                <span className="genre">{song.genre}</span>
                                <span className="likes">❤️ {song.likes}</span>
                            </div>
                        </div>
                        {expandedIndex === song.index && (
                            <div className="gallery-expanded">
                                <audio controls src={song.previewUrl} onClick={(e) => e.stopPropagation()}>
                                    Your browser does not support the audio element.
                                </audio>
                                <p className="review">{song.review}</p>
                            </div>
                        )}
                    </div>
                ))}
            </div>
            <div ref={loadMoreRef} className="load-more">
                {loading && <div className="spinner"></div>}
            </div>
        </div>
    );
};
