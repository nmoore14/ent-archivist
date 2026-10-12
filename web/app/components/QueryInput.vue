<script setup lang="ts">
// QueryInput.vue
/**
 * Component for handling user query submission.
 * This is the primary interaction area, emphasizing the query's role.
 */
import { ref } from 'vue';

// State for the input query
const searchQuery = ref('');
// State for loading/query submission status
const isSubmitting = ref(false);

/**
 * Handles the query submission logic.
 * This function will eventually be connected to the API call.
 * @param event - The submit event from the form.
 */
const handleSubmitQuery = async (event: Event) => {
  if (event) event.preventDefault();

  if (!searchQuery.value.trim()) {
    alert('Please enter a query to start.');
    return;
  }

  // In a real application:
  // 1. Set isSubmitting.value = true
  // 2. Call $fetch('/api/query', { method: 'POST', body: { query: searchQuery.value } })
  // 3. Handle response and display results.
  // 4. Set isSubmitting.value = false
  
  console.log('Submitting query:', searchQuery.value);
  alert(`[Simulation] Query submitted: "${searchQuery.value}". The backend will now perform federated search.`);
};
</script>

<template>
  <!-- Container for the input, giving it a dedicated, focused look. -->
  <div class="mt-16">
    <h2 class="text-xl font-semibold text-gray-800 mb-4">
      Ask Your Query
    </h2>
    
    <form @submit.prevent="handleSubmitQuery" class="flex gap-3">
      <!-- The primary input field must be visually commanding. -->
      <div class="flex-grow">
        <label for="queryInput" class="block text-sm font-medium text-gray-700 mb-1">
          Enter your query about the organization's knowledge.
        </label>
        <textarea 
          id="queryInput"
          v-model="searchQuery" 
          rows="3" 
          placeholder="e.g., What is the retention policy for customer data in the European region?" 
          class="w-full p-3 border-2 border-gray-300 rounded-lg focus:border-blue-500 focus:ring-blue-500 transition duration-150 resize-none shadow-md"
          :disabled="isSubmitting"
        ></textarea>
      </div>

      <!-- Submit button -->
      <button 
        type="submit" 
        :disabled="!searchQuery.trim() || isSubmitting"
        @click="handleSubmitQuery($event)"
        class="px-8 py-3 bg-blue-600 text-white font-semibold rounded-lg shadow-md hover:bg-blue-700 transition duration-150 disabled:bg-blue-300 disabled:cursor-not-allowed flex items-center"
      >
        <span v-if="!isSubmitting">
            Search Knowledge
        </span>
        <svg v-else class="animate-spin -ml-1 mr-2 h-5 w-5 text-white" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-80" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12h1C4 10.343 6.343 8 8 8V4h8v4c-1.657 0-3.96-.343-5 1.291z"></path>
        </svg>
        Searching...
      </button>
    </form>
  </div>
</template>

<style scoped>
/* Scoped styles for QueryInput */
</style>