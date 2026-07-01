import type { EventRecord, FaultPeriod } from '$lib/types';

const API_BASE = import.meta.env.VITE_CONTROLLER_HTTP_URL ?? 'http://localhost:5265';



export async function fetchEvents(params?: {
    from?: string;
    to?: string;
    source?: string;
}): Promise<EventRecord[]> {
    const query = new URLSearchParams();
    if (params?.from) query.set('from', params.from);
    if (params?.to) query.set('to', params.to);
    if (params?.source) query.set('source', params.source);

    const res = await fetch(`${API_BASE}/reports/events?${query}`);
    if (!res.ok) throw new Error('Failed to fetch events');
    return res.json();
}



export async function fetchFaultPeriods(): Promise<FaultPeriod[]> {
    const res = await fetch(`${API_BASE}/reports/faults`);
    if (!res.ok) throw new Error('Failed to fetch fault periods');
    return res.json();
}