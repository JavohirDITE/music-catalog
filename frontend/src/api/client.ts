import { SongsResponse } from '../types/Song';

const API_BASE = import.meta.env.VITE_API_URL || '';

export async function fetchSongs(
    locale: string,
    seed: string,
    likes: number,
    page: number,
    pageSize: number
): Promise<SongsResponse> {
    const params = new URLSearchParams({
        locale,
        seed,
        likes: likes.toString(),
        page: page.toString(),
        pageSize: pageSize.toString()
    });

    const response = await fetch(`${API_BASE}/api/songs?${params}`);
    if (!response.ok) {
        throw new Error('Failed to fetch songs');
    }
    return response.json();
}

export function getCoverUrl(locale: string, seed: string, page: number, pageSize: number, index: number): string {
    return `${API_BASE}/api/cover?locale=${locale}&seed=${seed}&page=${page}&pageSize=${pageSize}&index=${index}`;
}

export function getPreviewUrl(locale: string, seed: string, page: number, pageSize: number, index: number): string {
    return `${API_BASE}/api/preview?locale=${locale}&seed=${seed}&page=${page}&pageSize=${pageSize}&index=${index}`;
}
