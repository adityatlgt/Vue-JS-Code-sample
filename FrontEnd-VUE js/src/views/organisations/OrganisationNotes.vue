<template>
  <detail fill>
    <!-- <alp-heading heading="Organisation Notes" /> -->

    <div class="detail-inner-content flex flex-col overflow-y-auto">
      <alp-section>
        <alp-form-divider name="Add New Notes" />
        <div class="flex flex-col pt-1 pb-8 px-2" id="editorNoteOrganisation">
          <editor editor-container-class="max-h-64"  v-model="state.message" />
          <span class="flex justify-end mt-8">
            <alp-button
              class="w-1/7 mr-1 text-xs"
              variant="plain"
              @click="state.message = ''"
            >
              Clear
            </alp-button>
            <alp-button
              class="w-1/7 ml-1 text-xs"
              variant="inverse"
              @click="createNote"
            >
              File
            </alp-button>
          </span>
        </div>
      </alp-section>

      <alp-section class="flex-1 flex flex-col">
        <alp-form-divider name="Notes Records" />
        <span class="flex mx-2 mb-1 mt-3">
          <alp-focus-input
            class="flex-1 text-sm h-10"
            placeholder="Search (Press 'Ctrl + /' to focus)"
            v-model="state.search"
          />
        </span>

        <div
          class="flex-1 flex flex-col overflow-y-auto mt-3 mx-2 rounded-t-lg"
        >
          <span
            v-for="note in notes"
            :key="note.id"
            class="w-full flex pt-3"
            :class="{
              'pr-8': note.insertedBy?.id != currentUserId,
              'pl-20': note.insertedBy?.id == currentUserId
            }"
          >
            <span
              class="w-full flex flex-col my-1 rounded-sm"
              :class="{
                'border-l-4 border-indigo-500 ring-1 ring-gray-100': note.insertedBy?.id != currentUserId,
                'border-r-4 border-indigo-500 ring-1 ring-gray-100': note.insertedBy?.id == currentUserId
              }"
            >
              <div class="w-full flex justify-between font-medium">
                <span class="text-xs px-3 py-1 text-gray-500">
                  {{ note.insertedBy?.firstName }}
                  {{ note.insertedBy?.lastName }}
                </span>
                <span class="text-xs px-3 py-1 text-gray-500">
                  Created at: {{ fmtToLocalShortDate(note.insertedAt) }} ({{
                    fmtTimeDistance(note.insertedAt)
                  }})
                </span>
              </div>
              <div class="w-full flex justify-between font-medium">
                <span
                  class="prose prose-sm text-sm p-3"
                  v-html="note.message"
                />
                <div
                  @click="showEditOrganizationNoteModal(note.id)"
                  class="pointer mt-2 mr-2"
                >
                  <alp-icon icon="fa-solid fa-pen-to-square" size="16"></alp-icon>
                </div>
              </div>
            </span>
          </span>
        </div>
        <alp-paginator
          class="text-sm mx-6"
          v-model:limit="state.limit"
          v-model:offset="state.offset"
          :item-count="notes.length"
          :total="notesCount"
        />
      </alp-section>
    </div>
  </detail>
</template>

<script lang="ts">
import Editor from "@/components/inputs/Editor.vue";
import Detail from "@/components/ui/layout/Detail.vue";
import { fmtTimeDistance } from "@/composable/date";
import { fmtToLocalShortDate } from "@/composable/date";
import { NoteDtoPaginatedDto, NoteInput } from "@/network/crm-service-proxies";
import { AuthStore } from "@/store/modules/auth";
import { ModalStore, ModalType } from "@/store/modules/modals";
import { OrganisationStore } from "@/store/modules/organisations";
import { UserStore } from "@/store/modules/users";
import { ApiStore } from "@/store/utils";
import { computed, defineComponent, onMounted, reactive, watch } from "vue";
import { useStore } from "vuex";

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  components: {
    Detail,
    Editor
  },
  setup(props) {
    const store = useStore();
    const currentUserId = computed(
      () => store.getters[UserStore.getters.GET_ME]?.id
    );
    const notes = computed(
      () =>
        ApiStore.toData<NoteDtoPaginatedDto>(
          store.getters[OrganisationStore.getters.GET_ORGANISATION_NOTES]
        )?.items || []
    );
    const notesCount = computed(
      () =>
        ApiStore.toData<NoteDtoPaginatedDto>(
          store.getters[OrganisationStore.getters.GET_ORGANISATION_NOTES]
        )?.count || 0
    );

    const state = reactive({
      message: "",
      search: "",
      limit: 20,
      offset: 0
    });

    function fetchOrganisationNotes() {
      store.dispatch(OrganisationStore.actions.GET_ORGANISATION_NOTES, {
        id: props.id,
        limit: state.limit,
        offset: state.offset,
        search: state.search
      });
    }

    function showEditOrganizationNoteModal(noteId: number) {
      store.dispatch(ModalStore.actions.SHOW_MODAL, {
        modal: ModalType.EditOrganizationNote,
        props: {
          id: props.id,
          noteid: noteId
        }
      });
    }

    onMounted(fetchOrganisationNotes);

    watch(
      [() => state.limit, () => state.offset, () => state.search],
      fetchOrganisationNotes
    );

    function createNote() {
      if (state.message) {
        store
          .dispatch(OrganisationStore.actions.CREATE_ORGANISATION_NOTE, {
            id: props.id,
            note: new NoteInput({ message: state.message })
          })
          .then(() => (state.message = ""));
      }
    }

    return {
      state,
      currentUserId,
      notes,
      notesCount,
      fmtTimeDistance,
      fmtToLocalShortDate,
      createNote,
      showEditOrganizationNoteModal
    };
  }
});
</script>
