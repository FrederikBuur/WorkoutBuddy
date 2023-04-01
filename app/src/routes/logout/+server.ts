import { error, redirect } from "@sveltejs/kit";
import type { RequestHandler } from "./$types";

export const POST: RequestHandler = async ({ locals }) => {
	const { error: err } = await locals.sb.auth.signOut();

	if (err) {
		console.log("Something went wron logging out");
		throw error(500, "Somthing went wrong logging you out.");
	}

	throw redirect(303, "/");
};
