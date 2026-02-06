export interface Song {
    index: number;
    title: string;
    artist: string;
    album: string;
    genre: string;
    likes: number;
    review: string;
    coverUrl: string;
    previewUrl: string;
}

export interface SongsResponse {
    page: number;
    pageSize: number;
    items: Song[];
}
