<script lang="ts">
    import '../app.css';
    import ConnectionBanner from '$lib/components/ConnectionBanner.svelte';

    let {children} = $props();
</script>



<div class="min-h-screen flex flex-col bg-gray-50">
    <header class="bg-white border-b border-gray-200">
        <div class="px-4 py-3 flex items-center justify-between">
            <h1 class="text-base font-medium text-gray-900">M23 process monitor</h1>
        </div>
        <ConnectionBanner/>
    </header>

    <nav class="bg-white border-b border-gray-200">
        <div class="flex px-4">
            <a class="px-4 py-2 text-sm border-b-2 border-gray-900 font-medium text-gray-900" href="/">Process</a>
            <a class="px-4 py-2 text-sm border-b-2 border-transparent text-gray-500 hover:text-gray-700" href="/log">Event
                log</a>
            <a class="px-4 py-2 text-sm border-b-2 border-transparent text-gray-500 hover:text-gray-700"
               href="/reports">Reports</a>
        </div>
    </nav>

    <main class="flex-1 p-4">
        {@render children()}
    </main>
</div>