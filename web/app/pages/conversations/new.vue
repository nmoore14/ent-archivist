<script setup lang="ts">
import { ref } from "vue";

const route = useRoute();
const draft = ref(typeof route.query.question === "string" ? route.query.question : "");
const sent = ref(typeof route.query.question === "string");
const messages = ref<{ role: string; text: string }[]>(sent.value ? [{ role: "user", text: draft.value }] : []);

function send() {
  const value = draft.value.trim();
  if (!value) return;
  messages.value.push({ role: "user", text: value });
  draft.value = "";
  sent.value = true;
}
</script>

<template>
  <main class="conversation-page">
    <header class="conversation-header"><NuxtLink to="/" class="brand" aria-label="Archivist home"><VAvatar color="primary" size="34" rounded="lg"><VIcon icon="mdi-archive-arrow-down-outline" size="20" /></VAvatar><span>archivist</span></NuxtLink><div><VBtn to="/explore" variant="text" prepend-icon="mdi-compass-outline">Explore</VBtn><VBtn to="/" variant="text" icon="mdi-close" aria-label="Close conversation" /></div></header>
    <div class="conversation-shell">
      <section class="conversation-thread" aria-label="Conversation">
        <template v-if="!sent"><div class="thread-empty"><VAvatar size="48" color="primary" variant="tonal"><VIcon icon="mdi-sparkles" /></VAvatar><h1>Start with a question.</h1><p>Archivist will search the approved knowledge available to you and show the records behind every answer.</p></div></template>
        <template v-else><article v-for="message in messages" :key="message.text" class="message message--user"><p>{{ message.text }}</p></article><article class="message message--answer"><div class="answer-mark"><VIcon icon="mdi-sparkles" size="18" /></div><div><p class="answer-label">Preview answer</p><p>When chat is connected, this space will stream a grounded answer with citations. The source panel shows the evidence pattern the experience will use.</p><div class="citation-preview"><NuxtLink to="/sources/retention-policy"><VIcon icon="mdi-file-document-outline" color="primary" />Data retention standard <VIcon icon="mdi-arrow-up-right" size="16" /></NuxtLink><span>Section 4.2 · Retention schedules</span></div></div></article></template>
      </section>
      <aside v-if="sent" class="evidence-panel"><p class="page-overline">Evidence</p><h2>Sources used</h2><NuxtLink to="/sources/retention-policy" class="evidence-source"><VIcon icon="mdi-shield-check-outline" color="primary" /><span><strong>Data retention standard</strong><small>Policy · Updated 3 days ago</small></span><VIcon icon="mdi-chevron-right" /></NuxtLink><p class="evidence-note">Citations will only reference material your role can access.</p></aside>
      <VForm class="conversation-composer" @submit.prevent="send"><VTextarea v-model="draft" variant="solo" hide-details auto-grow rows="1" placeholder="Ask about approved knowledge…" aria-label="Ask Archivist" @keydown.meta.enter.prevent="send" @keydown.ctrl.enter.prevent="send" /><VBtn type="submit" color="primary" icon="mdi-send" :disabled="!draft.trim()" aria-label="Send question" /></VForm>
    </div>
  </main>
</template>
