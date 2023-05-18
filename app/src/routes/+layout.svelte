<script lang="ts">
	import "../app.css";
	import { invalidateAll } from "$app/navigation";
	import Nav from "$lib/components/nav.svelte";
	import { supabaseClient } from "$lib/supabase";
	import { onMount } from "svelte";

	onMount(() => {
		const {
			data: { subscription },
		} = supabaseClient.auth.onAuthStateChange(() => {
			console.log("Auth state changed");
			invalidateAll();
		});

		return () => {
			subscription.unsubscribe();
		};
	});
</script>

<Nav user="" />
<slot />

<style>
	/* @tailwind base;
	@tailwind components;
	@tailwind utilities; */
</style>
