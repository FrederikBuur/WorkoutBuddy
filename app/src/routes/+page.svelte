<script lang="ts">
	import { enhance, type SubmitFunction } from "$app/forms";
	import Nav from "$lib/components/nav.svelte";
	import { supabaseClient } from "$lib/supabase";
	import type { PageData } from "./$types";
	import { page } from "$app/stores";

	export let data: PageData;

	const submitLogout: SubmitFunction = async ({ cancel }) => {
		const { error } = await supabaseClient.auth.signOut();
		if (error) {
			console.log(error);
		}
		cancel();
	};

	let path;
	$: path = $page.url.pathname;
	console.log(path);
</script>

<div class="flex flex-col items-center gap-4">
	{#if data.session}
		<p>Welcome, {data.session.user.email}</p>
		<form action="/logout" method="post" use:enhance={submitLogout}>
			<button
				type="submit"
				class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-md"
				>Logout</button
			>
		</form>
	{:else}
		<h1 class="font-bold text-4xl">SvelteKit & Supabase Auth</h1>
		<p class="">Let's learn how to register and login users!</p>
		<div class="flex flex-row gap-4">
			<a
				href="/login"
				class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded-md">Login</a
			>
			<a
				href="/register"
				class="bg-teal-600 hover:bg-teal-700 text-white font-bold py-2 px-4 rounded-md">Register</a
			>
		</div>
	{/if}
</div>
