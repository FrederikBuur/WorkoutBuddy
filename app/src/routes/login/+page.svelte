<script lang="ts">
	import type { ActionData } from "./$types";

	export let form: ActionData;
	console.log(form);
</script>

<main class="flex flex-col items-center gap-4">
	<h1 class="font-bold text-4xl">Login</h1>
	<form action="?/login" method="post" class="flex flex-col w-96">
		<label for=""> Email </label>
		<input
			type="text"
			name="email"
			value={form?.email ?? ""}
			class="border-2 border-gray-300 p-2 rounded-md"
		/>
		<label for=""> Password </label>
		<input
			type="password"
			name="password"
			value={form?.password ?? ""}
			class="border-2 border-gray-300 p-2 rounded-md"
		/>
		<button
			type="submit"
			class="bg-blue-600 hover:bg-blue-700 text-white font-bold p-3 rounded-md my-6"
			>Login
		</button>
		{#if form?.error}
			<p class="py-2 px-4 font-sans text-white bg-red-400 rounded-xl content-center">
				{form?.error}
			</p>
		{/if}
	</form>
</main>
