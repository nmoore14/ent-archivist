import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

export default defineNuxtPlugin((nuxtApp) => {
  const vuetify = createVuetify({
    // Nuxt renders this app on the server as well as in the browser.  Tell
    // Vuetify to use its SSR-safe rendering path so slot rendering retains
    // the active Vue component instance.
    ssr: true,
    components,
    directives,
    icons: { defaultSet: 'mdi' },
    theme: { defaultTheme: 'archivist', themes: { archivist: { dark: false, colors: { background: '#f7f7f4', surface: '#ffffff', primary: '#4e5bd4', secondary: '#343a4a', 'on-surface': '#202330', 'surface-variant': '#f0f1f5' } } } },
  })
  nuxtApp.vueApp.use(vuetify)
})
