import { sveltekit } from "@sveltejs/kit/vite";
import type { UserConfig } from "vite";

const config: UserConfig = {
	plugins: [sveltekit()],
	// force https + set port?
	// server: {
	//     https: true,
	//     port: 6363
	// },
};

export default config;
