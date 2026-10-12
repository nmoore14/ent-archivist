<script setup lang="ts">
import { computed } from 'vue'

const config = useRuntimeConfig()
const apiBase = computed(() => config.public.apiBase ?? 'http://localhost:5080')
</script>

<template>
  <div class="status-card p-6 bg-white shadow-lg rounded-xl border border-gray-100">
    <div class="flex items-center justify-between gap-4">
      <h3 class="text-lg font-semibold text-gray-700">System Connectivity Status</h3>
      <span class="text-sm font-mono bg-gray-100 px-3 py-1 rounded-full text-gray-600">
        {{ apiBase }}
      </span>
    </div>

    <div class="mt-4 pt-4 border-t border-gray-100">
      <p class="text-sm text-gray-500 mb-2">
        This chat service performs federated search across all workspaces
        for which your identity possesses an active <code>chat.query</code> permission.
      </p>
      <p class="text-sm font-medium text-gray-600">
        All queries are auditable and constrained by governance policies.
      </p>
    </div>
  </div>
</template>

<style scoped>
.status-card {
  transition: all 0.3s ease;
}
/* Placeholder styles for structure */
</style>