<script setup lang="ts">
import { computed, ref } from "vue";

const folders = ["Board materials", "Product research", "Policy library"];
const history = ref([
  "Q3 planning notes",
  "Vendor renewal terms",
  "Customer interview synthesis",
]);
const recentChats = ref([
  {
    title: "Board materials",
    detail: "Updated yesterday",
    icon: "mdi-file-document-outline",
  },
  {
    title: "Retention policy review",
    detail: "Updated Monday",
    icon: "mdi-shield-check-outline",
  },
  {
    title: "Research synthesis",
    detail: "Updated last week",
    icon: "mdi-text-box-search-outline",
  },
]);
const suggestions = [
  { label: "Find a document", icon: "mdi-file-find-outline" },
  { label: "Compare policies", icon: "mdi-compare" },
  { label: "Summarize a brief", icon: "mdi-text-short" },
  { label: "Prepare a question", icon: "mdi-pencil-outline" },
];

const question = ref("");
const mobileMenuOpen = ref(false);
const notice = ref("");
const noticeVisible = ref(false);
const activeNav = ref("Home");
const showContextInsight = ref(true);
const input = ref<HTMLTextAreaElement | null>(null);
const canSubmit = computed(() => question.value.trim().length > 0);

function showNotice(message: string) {
  notice.value = message;
  noticeVisible.value = true;
}

function startConversation() {
  mobileMenuOpen.value = false;
  navigateTo("/conversations/new");
}

function chooseSuggestion(label: string) {
  question.value = `${label}: `;
  requestAnimationFrame(() => input.value?.focus());
}

function submitQuestion() {
  const submittedQuestion = question.value.trim();
  if (!submittedQuestion) return;

  navigateTo({ path: "/conversations/new", query: { question: submittedQuestion } });
}

function selectNav(item: string) {
  activeNav.value = item;
  mobileMenuOpen.value = false;
  navigateTo(item === "Home" ? "/" : "/explore");
}
</script>

<template>
  <VLayout class="home-layout">
    <VAppBar class="mobile-app-bar" elevation="0" color="transparent">
      <VBtn
        icon="mdi-menu"
        variant="text"
        aria-label="Open navigation"
        @click="mobileMenuOpen = true"
      />
      <NuxtLink to="/" class="mobile-brand" aria-label="Archivist home"
        >archivist</NuxtLink
      >
      <VBtn
        icon="mdi-plus"
        variant="text"
        aria-label="Start a new conversation"
        @click="startConversation"
      />
    </VAppBar>

    <VNavigationDrawer v-model="mobileMenuOpen" temporary class="mobile-drawer">
      <div class="mobile-drawer__content">
        <NuxtLink to="/" class="brand" aria-label="Archivist home">
          <VAvatar color="primary" size="34" rounded="lg"
            ><VIcon icon="mdi-archive-arrow-down-outline" size="20"
          /></VAvatar>
          <span>archivist</span>
        </NuxtLink>
        <VBtn
          block
          size="large"
          variant="outlined"
          prepend-icon="mdi-plus"
          class="new-chat"
          @click="startConversation"
          >New conversation</VBtn
        >
        <VList nav density="comfortable" class="nav-list">
          <VListItem
            :active="activeNav === 'Home'"
            color="primary"
            prepend-icon="mdi-home-variant-outline"
            title="Home"
            @click="selectNav('Home')"
          />
          <VListItem
            :active="activeNav === 'Explore knowledge'"
            prepend-icon="mdi-compass-outline"
            title="Explore knowledge"
            @click="selectNav('Explore knowledge')"
          />
        </VList>
        <div class="sidebar-section">
          <div class="section-label">Recent</div>
          <VList density="compact" class="folder-list"
            ><VListItem
              v-for="item in history"
              :key="item"
              prepend-icon="mdi-message-text-outline"
              :title="item"
              @click="showNotice(`Opening ${item}`)"
          /></VList>
        </div>
      </div>
    </VNavigationDrawer>

    <VNavigationDrawer permanent width="284" class="sidebar">
      <div class="sidebar__content">
        <NuxtLink to="/" class="brand" aria-label="Archivist home">
          <VAvatar color="primary" size="34" rounded="lg"
            ><VIcon icon="mdi-archive-arrow-down-outline" size="20"
          /></VAvatar>
          <span>archivist</span>
        </NuxtLink>
        <VBtn
          block
          size="large"
          variant="outlined"
          prepend-icon="mdi-plus"
          class="new-chat"
          @click="startConversation"
          >New conversation</VBtn
        >
        <VList nav density="comfortable" class="nav-list">
          <VListItem
            :active="activeNav === 'Home'"
            color="primary"
            prepend-icon="mdi-home-variant-outline"
            title="Home"
            @click="selectNav('Home')"
          />
          <VListItem
            prepend-icon="mdi-compass-outline"
            title="Explore knowledge"
            :active="activeNav === 'Explore knowledge'"
            @click="selectNav('Explore knowledge')"
          />
        </VList>
        <div class="sidebar-section">
          <div class="section-label">
            <span>Collections</span
            ><VBtn
              icon="mdi-plus"
              variant="text"
              size="x-small"
              aria-label="Add collection"
              @click="
                showNotice('Collections are managed by your administrator.')
              "
            />
          </div>
          <VList density="compact" class="folder-list"
            ><VListItem
              v-for="folder in folders"
              :key="folder"
              prepend-icon="mdi-folder-outline"
              :title="folder"
              @click="showNotice(`Viewing ${folder}`)"
          /></VList>
        </div>
        <div class="sidebar-section">
          <div class="section-label">Recent</div>
          <VList density="compact" class="folder-list"
            ><VListItem
              v-for="item in history"
              :key="item"
              prepend-icon="mdi-message-text-outline"
              :title="item"
              @click="showNotice(`Opening ${item}`)"
          /></VList>
        </div>
      </div>
      <template #append>
        <div class="sidebar__footer">
          <VDivider class="mb-3" />
          <VListItem
            title="Nick Moore"
            subtitle="Enterprise workspace"
            rounded="lg"
            @click="showNotice('Profile settings are being prepared.')"
            ><template #prepend
              ><VAvatar color="primary" variant="tonal" size="32"
                >NM</VAvatar
              ></template
            ><template #append
              ><VIcon icon="mdi-dots-horizontal" size="20" /></template
          ></VListItem>
        </div>
      </template>
    </VNavigationDrawer>

    <VMain class="main-pane">
      <VContainer fluid class="page-frame">
        <div class="top-actions">
          <VChip
            size="small"
            variant="tonal"
            color="secondary"
            prepend-icon="mdi-shield-check-outline"
            >Governed workspace</VChip
          ><VBtn
            icon="mdi-dots-horizontal"
            variant="text"
            aria-label="More options"
            @click="showNotice('More workspace options are coming soon.')"
          />
        </div>
        <section class="chat-home" aria-labelledby="welcome-heading">
          <div class="welcome-mark">
            <VIcon icon="mdi-sparkles" size="22" />
          </div>
          <p class="workspace-name">ARCHIVIST / KNOWLEDGE WORKSPACE</p>
          <h1 id="welcome-heading">Good afternoon, Nick.</h1>
          <p class="welcome-copy">What would you like to understand today?</p>
          <VForm @submit.prevent="submitQuestion">
            <VCard class="ps-4 prompt-card" elevation="0" rounded="xl">
              <VTextarea
                ref="input"
                v-model="question"
                label="Ask Archivist"
                placeholder="Search approved knowledge, policies, and decisions…"
                variant="plain"
                rows="3"
                no-resize
                hide-details
                aria-label="Ask Archivist"
                @keydown.meta.enter.prevent="submitQuestion"
                @keydown.ctrl.enter.prevent="submitQuestion"
              />
              <VCardActions class="prompt-actions"
                ><VBtn
                  size="small"
                  variant="tonal"
                  color="secondary"
                  append-icon="mdi-chevron-down"
                  @click="
                    showNotice(
                      'Questions search only sources your current permissions allow.',
                    )
                  "
                  >All approved sources</VBtn
                ><VSpacer /><VBtn
                  icon="mdi-paperclip"
                  variant="text"
                  aria-label="Attach a file"
                  @click="
                    showNotice(
                      'Attachments will be available when upload is connected.',
                    )
                  " /><VBtn
                  icon="mdi-send"
                  color="primary"
                  variant="flat"
                  aria-label="Send question"
                  type="submit"
                  :disabled="!canSubmit"
              /></VCardActions>
            </VCard>
          </VForm>
          <div class="suggestions" aria-label="Suggested tasks">
            <VBtn
              v-for="suggestion in suggestions"
              :key="suggestion.label"
              size="small"
              variant="tonal"
              color="secondary"
              :prepend-icon="suggestion.icon"
              @click="chooseSuggestion(suggestion.label)"
              >{{ suggestion.label }}</VBtn
            >
          </div>
          <div class="home-insights">
            <VCard
              v-if="showContextInsight"
              variant="outlined"
              rounded="xl"
              class="insight-card"
              ><VCardText
                ><div class="d-flex align-start justify-space-between">
                  <div>
                    <p class="card-kicker">Workspace activity</p>
                    <p class="activity-count">18</p>
                    <p class="activity-caption">documents read this week</p>
                  </div>
                  <VIcon
                    icon="mdi-chart-timeline-variant"
                    color="primary"
                    size="30"
                  /></div></VCardText
            ></VCard>
            <VCard variant="outlined" rounded="xl" class="insight-card"
              ><VCardText class="d-flex ga-4"
                ><VAvatar color="primary" variant="tonal" rounded="lg"
                  ><VIcon icon="mdi-brain"
                /></VAvatar>
                <div>
                  <p class="card-kicker">Context-aware answers</p>
                  <p class="context-copy">
                    Answers cite the knowledge and decisions your role can
                    access.
                  </p>
                </div>
                <VBtn
                  icon="mdi-close"
                  size="x-small"
                  variant="text"
                  aria-label="Dismiss"
                  @click="showContextInsight = false" /></VCardText
            ></VCard>
          </div>
          <div class="recent-heading">
            <VIcon icon="mdi-message-text-outline" size="21" /><span
              >Your recent conversations</span
            ><VIcon icon="mdi-chevron-right" size="19" />
          </div>
          <div class="recent-grid">
            <VCard
              v-for="chat in recentChats"
              :key="chat.title"
              variant="outlined"
              rounded="xl"
              class="recent-card"
              tabindex="0"
              @click="showNotice(`Opening ${chat.title}`)"
              @keydown.enter="showNotice(`Opening ${chat.title}`)"
              ><VCardText
                ><VIcon
                  :icon="chat.icon"
                  color="primary"
                  size="23"
                  class="mb-8"
                />
                <p class="recent-title">{{ chat.title }}</p>
                <p class="recent-detail">{{ chat.detail }}</p></VCardText
              ></VCard
            >
          </div>
        </section>
      </VContainer>
    </VMain>
    <VSnackbar
      v-model="noticeVisible"
      location="bottom right"
      timeout="3500"
      color="primary"
      rounded="lg"
    >
      {{ notice }}
      <template #actions
        ><VBtn
          variant="text"
          aria-label="Dismiss notification"
          @click="noticeVisible = false"
          >Close</VBtn
        ></template
      >
    </VSnackbar>
  </VLayout>
</template>
