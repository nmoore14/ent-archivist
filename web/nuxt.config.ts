import vuetify from 'vite-plugin-vuetify'

export default defineNuxtConfig({
  compatibilityDate: '2026-10-09',
  css: ['~/assets/main.css'],
  devtools: { enabled: true },
  typescript: {
    strict: true,
    typeCheck: true,
  },
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE ?? 'http://localhost:5080',
    },
  },
  build: {
    transpile: ['vuetify'],
  },
  vite: {
    plugins: [vuetify({ autoImport: true })],
    ssr: {
      noExternal: ['vuetify'],
    },
  },
})
