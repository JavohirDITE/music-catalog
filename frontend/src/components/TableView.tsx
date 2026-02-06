import React, { useState } from 'react';
import { Song } from '../types/Song';

interface TableViewProps {
    songs: Song[];
    page: number;
    pageSize: number;
    onPageChange: (page: number) => void;
    onPageSizeChange: (pageSize: number) => void;
}

export const TableView: React.FC<TableViewProps> = ({
    songs,
    page,
    pageSize,
    onPageChange,
    onPageSizeChange
}) => {
    const [expandedIndex, setExpandedIndex] = useState<number | null>(null);

    const toggleExpand = (index: number) => {
        setExpandedIndex(expandedIndex === index ? null : index);
    };

    return (
        <div className="table-view">
            <table>
                <thead>
                    <tr>
                        <th style={{ width: '50px' }}>#</th>
                        <th>Song</th>
                        <th>Artist</th>
                        <th>Album</th>
                        <th>Genre</th>
                        <th style={{ width: '80px' }}>Likes</th>
                    </tr>
                </thead>
                <tbody>
                    {songs.map((song) => (
                        <React.Fragment key={song.index}>
                            <tr
                                onClick={() => toggleExpand(song.index)}
                                className={expandedIndex === song.index ? 'expanded' : ''}
                            >
                                <td>
                                    <span className={`expand-icon ${expandedIndex === song.index ? 'rotated' : ''}`}>
                                        ›
                                    </span>
                                    {song.index}
                                </td>
                                <td className="song-title">{song.title}</td>
                                <td className="artist-name">{song.artist}</td>
                                <td className="album-name">{song.album}</td>
                                <td><span className="genre-tag">{song.genre}</span></td>
                                <td className="likes-cell">❤️ {song.likes}</td>
                            </tr>
                            {expandedIndex === song.index && (
                                <tr className="expanded-row">
                                    <td colSpan={6}>
                                        <div className="expanded-content">
                                            <div className="cover-section">
                                                <img src={song.coverUrl} alt={song.title} className="cover-image" />
                                                <div className="likes-badge">
                                                    <span>❤️</span>
                                                    <span>{song.likes}</span>
                                                </div>
                                            </div>
                                            <div className="song-details">
                                                <div className="song-header">
                                                    <h3>{song.title}</h3>
                                                    <audio controls className="audio-player" src={song.previewUrl}>
                                                        Your browser does not support the audio element.
                                                    </audio>
                                                </div>
                                                <p className="song-meta">
                                                    from <span className="album-link">{song.album}</span> by <span className="artist-link">{song.artist}</span>
                                                </p>
                                                <div className="review-section">
                                                    <h4>Review</h4>
                                                    <p className="review-text">{song.review}</p>
                                                </div>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            )}
                        </React.Fragment>
                    ))}
                </tbody>
            </table>

            <div className="pagination">
                <button onClick={() => onPageChange(page - 1)} disabled={page <= 1}>
                    «
                </button>
                <div className="page-info">
                    <span className="page-number">{page}</span>
                </div>
                <button onClick={() => onPageChange(page + 1)}>
                    »
                </button>
                <select value={pageSize} onChange={(e) => onPageSizeChange(parseInt(e.target.value))}>
                    <option value="10">10 / page</option>
                    <option value="20">20 / page</option>
                    <option value="50">50 / page</option>
                </select>
            </div>
        </div>
    );
};
