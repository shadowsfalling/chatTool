import { createApp } from 'vue'

// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { createPinia } from 'pinia';
import router from './router';

// Components
import App from './App.vue'

const vuetify = createVuetify({
    components,
    directives,
})

createApp(App)
.use(createPinia())
.use(router)
.use(vuetify)
.mount('#app')