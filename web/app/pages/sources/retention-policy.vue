<script setup lang="ts">
import { ref } from "vue";

const copied = ref(false);
async function copyCitation() {
  await navigator.clipboard.writeText(
    "Data retention standard, version 3.1, section 4.2: Retention schedules.",
  );
  copied.value = true;
  setTimeout(() => { copied.value = false; }, 1800);
}
</script>

<template>
  <main class="source-page">
    <header class="source-header"><NuxtLink to="/" class="brand" aria-label="Archivist home"><VAvatar color="primary" size="34" rounded="lg"><VIcon icon="mdi-archive-arrow-down-outline" size="20" /></VAvatar><span>archivist</span></NuxtLink><VBtn to="/explore" variant="text" prepend-icon="mdi-arrow-left">Back to Explore</VBtn></header>
    <article class="source-article"><div class="source-article__meta"><VChip color="primary" variant="tonal">Policy</VChip><span>Version 3.1</span><span>Updated 3 days ago</span></div><h1>Data retention standard</h1><p class="source-lead">The organization-wide standard for retaining, reviewing, and disposing of customer and operational information.</p><VDivider class="my-8" /><section><h2>4.2 Retention schedules</h2><p>Data owners must apply the approved schedule for each information class. A legal hold pauses routine deletion until the hold is released by Legal.</p><blockquote>Customer account records are retained for seven years after account closure, unless a longer period is required by a legal hold, regulatory obligation, or an approved exception.</blockquote><p>Systems that cannot apply the schedule automatically must have an approved manual review process and an accountable owner.</p></section><aside class="source-callout"><VIcon icon="mdi-shield-check-outline" color="primary" /><div><strong>Access checked</strong><p>This source is shown because it is available to your current role.</p></div></aside><div class="source-actions"><VBtn variant="tonal" prepend-icon="mdi-content-copy" @click="copyCitation">{{ copied ? 'Citation copied' : 'Copy citation' }}</VBtn><VBtn to="/conversations/new" color="primary" variant="flat" prepend-icon="mdi-message-text-outline">Ask about this policy</VBtn></div></article>
  </main>
</template>
