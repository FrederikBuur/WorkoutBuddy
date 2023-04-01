<script lang="ts">
</script>

<main class="flex flex-col items-center gap-4">
	<h1 class="font-bold text-4xl">Register</h1>
	<form action="?/register" method="post" class="flex flex-col w-96">
		<label for=""> Email </label>
		<input type="text" name="email" class="border-2 border-gray-300 px-2 py-1 rounded-md" />
		<label for=""> Password </label>
		<input type="password" name="password" class="border-2 border-gray-300 px-2 py-1 rounded-md" />
		<button class="bg-teal-600 hover:bg-teal-700 text-white font-bold p-3 rounded-md mt-6"
			>Register
		</button>
	</form>
</main>
