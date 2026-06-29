<script lang="ts">
    import { connection } from '$lib/stores/connection.svelte';
    import { fetchEvents } from '$lib/api';
    import type { EventRecord } from '$lib/types';
    import { onMount } from 'svelte';


    let historical = $state<EventRecord[]>([]);
    let loading = $state(true);
    let error = $state<string | null>(null);
    let sourceFilter = $state<string>('');

    const sources = ['', 'M1', 'M2', 'M3', 'M4', 'FLAP', 'SYSTEM', 'ALARM'];

    const filteredLiveEvents = $derived(
        sourceFilter
            ? connection.events.filter(e => e.source === sourceFilter)
            : connection.events
    );

    async function loadEvents() {
        loading = true;
        error = null;
        try {
            historical = await fetchEvents(sourceFilter ? { source: sourceFilter } : undefined);
        } catch (e) {
            error = e instanceof Error ? e.message : 'Failed to load events';
        } finally {
            loading = false;
        }
    }

    onMount(loadEvents);

    function formatTime(value: string | Date): string {
        const d = typeof value === 'string' ? new Date(value) : value;
        return d.toLocaleString();
    }
</script>



<div class="max-w-3xl mx-auto space-y-4">
    <div class="flex items-center justify-between">
        <h2 class="text-base font-medium text-gray-900">Event log</h2>
        <div class="flex items-center gap-2">
            <select
                    bind:value={sourceFilter}
                    onchange={loadEvents}
                    class="text-xs border border-gray-300 rounded px-2 py-1"
            >
                {#each sources as s}
                    <option value={s}>{s || 'All sources'}</option>
                {/each}
            </select>
            <button
                    onclick={loadEvents}
                    class="text-xs px-2 py-1 border border-gray-300 rounded hover:bg-gray-50"
            >
                Refresh
            </button>
        </div>
    </div>

    {#if filteredLiveEvents.length > 0}
        <div>
            <p class="text-xs text-gray-500 mb-1.5">Live (this session)</p>
            <div class="bg-white border border-gray-200 rounded-lg divide-y divide-gray-100">
                {#each filteredLiveEvents as event}
                    <div class="px-3 py-2 flex items-center justify-between text-sm">
                        <span class="font-medium text-gray-900">{event.source}</span>
                        <span class="text-gray-500">→ {event.to}</span>
                        <span class="text-xs text-gray-400">{formatTime(event.timestamp)}</span>
                    </div>
                {/each}
            </div>
        </div>
    {/if}

    <div>
        <p class="text-xs text-gray-500 mb-1.5">History</p>
        {#if loading}
            <p class="text-sm text-gray-400 py-4 text-center">Loading…</p>
        {:else if error}
            <p class="text-sm text-red-600 py-4 text-center">{error}</p>
        {:else if historical.length === 0}
            <p class="text-sm text-gray-400 py-4 text-center">No events recorded yet</p>
        {:else}
            <div class="bg-white border border-gray-200 rounded-lg divide-y divide-gray-100">
                {#each historical as event}
                    <div class="px-3 py-2 flex items-center justify-between text-sm">
                        <span class="font-medium text-gray-900">{event.source}</span>
                        <span class="text-gray-500">{event.from} → {event.to}</span>
                        <span class="text-xs text-gray-400">{formatTime(event.timestamp)}</span>
                    </div>
                {/each}
            </div>
        {/if}
    </div>
</div>