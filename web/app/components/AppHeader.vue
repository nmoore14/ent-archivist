<script setup lang="ts">
// AppHeader.vue
/**
 * Component responsible for the main title and branding.
 * This section communicates the system's unique value proposition:
 * accessing auditable, governed knowledge, not just a general AI chat.
 */

// Props could be used here if we need to dynamically change the title/subtitle
</script>

<template>
  <div class="max-w-3xl">
    <!-- The main title should be highly visible and assertive. -->
    <h1 class="text-4xl font-extrabold tracking-tight text-gray-900 sm:text-6xl">
      Query your organization's approved, auditable knowledge base.
    </h1>
    
    <!-- Subtitle should explain the mechanism (federated search) and limitation (no workspace selection UI). -->
    <p class="mt-4 text-xl text-gray-500 max-w-xl">
      The system performs federated searches across all workspaces for which your identity has effective <code class="bg-yellow-100 p-1 rounded">chat.query</code> permission.
    </p>
    
    <!-- Disclaimer box to reinforce security/governance -->
    <div class="mt-10 p-4 bg-blue-50 border-l-4 border-blue-500 text-blue-800 rounded-md shadow-sm"
         role="alert">
      <div class="flex items-center">
        <svg class="flex-shrink-0 w-5 h-5 mr-3" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <p class="text-sm font-medium">
          ⚠️ Governance Notice: Workspaces are managed by administrators. You will not select a workspace in this UI; access is determined entirely by your current user permissions.
        </p>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Scoped styles for AppHeader component */
</style>