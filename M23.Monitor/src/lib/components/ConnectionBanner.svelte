<script lang="ts">
    import { connection } from '$lib/stores/connection.svelte';

    const defaultWsUrl = import.meta.env.VITE_CONTROLLER_WS_URL ?? 'ws://localhost:5265/ws';
    let url = $state(defaultWsUrl);

    function handleConnect() {
        connection.connect(url);
    }
</script>



<div class="flex items-center gap-3 px-4 py-2 border-b border-gray-200 bg-white text-sm">
    <span class="text-gray-500">Server</span>
    <input
            type="text"
            bind:value={url}
            class="flex-1 border border-gray-200 rounded px-2 py-1 text-sm font-mono"
            placeholder="ws://<address>:<port>/ws"
    />
    <button
            onclick={handleConnect}
            class="px-3 py-1 border border-gray-300 rounded hover:bg-gray-50 text-sm"
    >
        {connection.status === 'connected' ? 'Reconnect' : 'Connect'}
    </button>
    <span class="text-xs px-2 py-0.5 rounded-full font-medium
        {connection.status === 'connected' ? 'bg-green-100 text-green-800' :
         connection.status === 'connecting' ? 'bg-amber-100 text-amber-800' :
         connection.status === 'error' ? 'bg-red-100 text-red-800' :
         'bg-gray-100 text-gray-600'}">
    {connection.status}
  </span>
</div>