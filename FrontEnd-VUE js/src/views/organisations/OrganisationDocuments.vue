<template>
  <detail fill>
    <!-- <alp-heading heading="Documents" /> -->
    <div class="detail-inner-content flex flex-col overflow-y-auto">
      <document-list
        class="flex-1 mt-8"
        :loading="loading"
        :uploading="state.uploading"
        :documents="items"
        :documents-count="count"
        :can-create="can('OrganisationDocument.Create')"
        :can-edit="can('OrganisationDocument.Edit')"
        :can-delete="can('OrganisationDocument.Delete')"
        @fetch="fetch"
        @upload="uploadOrganisationDocuments"
        @create-from-resource="createFromResource"
      />
    </div>
  </detail>
</template>

<script lang="ts">
import DocumentList from "@/components/ui/documents/DocumentList.vue";
import Detail from "@/components/ui/layout/Detail.vue";
import { useCan } from "@/composable/can";
import { useListable } from "@/composable/listable";
import { DocumentDto } from "@/network/documents-service-proxies";
import { OrganisationStore } from "@/store/modules/organisations";
import { defineComponent, reactive } from "vue";
import { useStore } from "vuex";

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  components: {
    DocumentList,
    Detail
  },
  setup(props) {
    const store = useStore();
    const { can } = useCan();

    const state = reactive({
      uploading: false
    });

    const { items, count, loading, fetch } = useListable({
      items: OrganisationStore.getters.GET_ORGANISATION_DOCUMENTS,
      query: OrganisationStore.actions.GET_ORGANISATION_DOCUMENTS,
      queryParams: () => ({
        id: props.id
      })
    });

    function uploadOrganisationDocuments(files: Array<File>) {
      store.dispatch(OrganisationStore.actions.UPLOAD_ORGANISATION_DOCUMENTS, {
        id: props.id,
        files
      });
    }

    function createFromResource({ resource }: { resource: DocumentDto }) {
      store.dispatch(OrganisationStore.actions.CREATE_DOCUMENT_FROM_RESOURCE, {
        id: props.id,
        resource
      });
    }

    return {
      can,
      state,
      items,
      count,
      loading,
      fetch,
      uploadOrganisationDocuments,
      createFromResource
    };
  }
});
</script>
