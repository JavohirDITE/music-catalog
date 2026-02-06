import React from 'react';

interface ToolbarProps {
    locale: string;
    seed: string;
    likes: number;
    view: 'table' | 'gallery';
    onLocaleChange: (locale: string) => void;
    onSeedChange: (seed: string) => void;
    onLikesChange: (likes: number) => void;
    onViewChange: (view: 'table' | 'gallery') => void;
    onRandomSeed: () => void;
}

export const Toolbar: React.FC<ToolbarProps> = ({
    locale,
    seed,
    likes,
    view,
    onLocaleChange,
    onSeedChange,
    onLikesChange,
    onViewChange,
    onRandomSeed
}) => {
    return (
        <div className="toolbar">
            <div className="toolbar-group">
                <label>Language</label>
                <select value={locale} onChange={(e) => onLocaleChange(e.target.value)}>
                    <option value="en-US">English (US)</option>
                    <option value="de-DE">Deutsch (DE)</option>
                </select>
            </div>

            <div className="toolbar-group">
                <label>Seed</label>
                <div className="control-row">
                    <input
                        type="text"
                        value={seed}
                        onChange={(e) => onSeedChange(e.target.value)}
                        placeholder="Enter seed"
                    />
                    <button onClick={onRandomSeed} title="Random seed">🎲</button>
                </div>
            </div>

            <div className="toolbar-group">
                <label>Likes</label>
                <div className="control-row">
                    <input
                        type="range"
                        min="0"
                        max="10"
                        step="0.1"
                        value={likes}
                        onChange={(e) => onLikesChange(parseFloat(e.target.value))}
                    />
                    <span className="likes-value">{likes.toFixed(1)}</span>
                </div>
            </div>

            <div className="view-buttons">
                <button
                    className={view === 'table' ? 'active' : ''}
                    onClick={() => onViewChange('table')}
                    title="Table view"
                >
                    ☰
                </button>
                <button
                    className={view === 'gallery' ? 'active' : ''}
                    onClick={() => onViewChange('gallery')}
                    title="Gallery view"
                >
                    ▦
                </button>
            </div>
        </div>
    );
};
