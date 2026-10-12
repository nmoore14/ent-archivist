<script setup lang="ts">
import { computed, ref } from "vue";

const query = ref("");
const selectedKind = ref("All");
const kinds = ["All", "Policy", "Decision", "Research"];
const documents = [
  { title: "Data retention standard", kind: "Policy", updated: "Updated 3 days ago", excerpt: "Defines approved retention periods, legal holds, and deletion responsibilities across customer data.", icon: "mdi-shield-check-outline", route: "/sources/retention-policy" },
  { title: "FY26 vendor renewal decision", kind: "Decision", updated: "Updated 12 Sep", excerpt: "Records the approved approach for critical vendor renewals and the review conditions that apply.", icon: "mdi-gavel", route: "/sources/retention-policy" },
  { title: "Customer research synthesis", kind: "Research", updated: "Updated 28 Aug", excerpt: "Themes from recent interviews, with source links and confidence notes for each finding.", icon: "mdi-text-box-search-outline", route: "/sources/retention-policy" },
];
const visibleDocuments = computed(() => documents.filter((document) => {
  const matchesKind = selectedKind.value === "All" || document.kind === selectedKind.value;
  const term = query.value.trim().toLowerCase();
  return matchesKind && (!term || `${document.title} ${document.excerpt}`.toLowerCase().includes(term));
}));
</script>

<template>
  <main class="support-page">
    <header class="support-header">
      <NuxtLink to="/" class="brand" aria-label="Archivist home"><VAvatar color="primary" size="34" rounded="lg"><VIcon icon="mdi-archive-arrow-down-outline" size="20" /></VAvatar><span>archivist</span></NuxtLink>
      <div class="support-header__actions"><VBtn to="/" variant="text" prepend-icon="mdi-home-variant-outline">Home</VBtn><VBtn to="/conversations/new" color="primary" variant="flat" prepend-icon="mdi-plus">New conversation</VBtn></div>
    </header>
    <section class="explore-hero" aria-labelledby="explore-heading">
      <p class="page-overline">Approved knowledge</p>
      <h1 id="explore-heading">Find the record behind the answer.</h1>
      <p>Browse only material available to your role. Results are local examples until search is connected.</p>
      <VTextField v-model="query" hide-details label="Search approved knowledge" placeholder="Try “retention” or “vendor”" prepend-inner-icon="mdi-magnify" variant="solo" class="explore-search" />
    </section>
    <section class="explore-results" aria-live="polite">
      <div class="filter-row"><span>{{ visibleDocuments.length }} records</span><VBtnToggle v-model="selectedKind" mandatory density="compact" color="primary"><VBtn v-for="kind in kinds" :key="kind" :value="kind">{{ kind }}</VBtn></VBtnToggle></div>
      <div v-if="visibleDocuments.length" class="document-list"><NuxtLink v-for="document in visibleDocuments" :key="document.title" :to="document.route" class="document-row"><VAvatar color="primary" variant="tonal" rounded="lg"><VIcon :icon="document.icon" /></VAvatar><div class="document-row__body"><div class="document-row__meta"><VChip size="x-small" variant="tonal">{{ document.kind }}</VChip><span>{{ document.updated }}</span></div><h2>{{ document.title }}</h2><p>{{ document.excerpt }}</p></div><VIcon icon="mdi-arrow-right" /></NuxtLink></div>
      <div v-else class="empty-state"><VIcon icon="mdi-file-search-outline" size="38" /><h2>No approved records match that search.</h2><p>Try a broader phrase or clear a filter.</p><VBtn variant="tonal" @click="query = ''; selectedKind = 'All'">Clear search</VBtn></div>
    </section>
  </main>
</template>
