<template>
  <detail fill>
    <!-- <alp-heading heading="Relationships"> -->

    <div class="detail-inner-content flex flex-col overflow-y-auto p-1">
      <span class="flex justify-between pb-3">
        <alp-focus-input
          class="flex-1 flex text-sm"
          placeholder="Search (Press 'Ctrl + /' to focus)"
          v-model="state.search"
        />
        <alp-button
          class="text-xs ml-3"
          @click="state.showCreateRelationshipModal = true"
          variant="primary"
          id="createRelationshipModal"
        >
          <alp-icon icon="fa-solid fa-plus" class="mr-2" size="14" />
          <span> Relationship </span>
        </alp-button>
      </span>

      <alp-infinite-table
        class="flex-1"
        :headers="['Organisation Name', 'Relationship Type', 'Relationship']"
        :fields="[
          'primaryOrganisationFullName',
          'organisation_relationshipType',
          'organisation_relationship'
        ]"
        :loading="loading"
        :values="items"
        @selected="
          $router.push({
            name: 'Organisation',
            params: { id: $event.primaryOrganisationId }
          })
        "
        @load-more="fetch"
      >
        <template v-slot:organisation_relationshipType="{ value }">
          <td>{{ toRelationshipType(value.organisation_relationshipType) }}</td>
        </template>
        <template v-slot:organisation_relationship="{ value }">
          <td>{{ toRelationships(value.organisation_relationship) }}</td>
        </template>
      </alp-infinite-table>
      <show-create-relationship
        v-if="state.showCreateRelationshipModal"
        :id="id"
        @close="closeCreate()"
      />
    </div>
  </detail>
</template>

<script lang="ts">
import AlpButton from "@/components/common/AlpButton.vue";
import Detail from "@/components/ui/layout/Detail.vue";
import showCreateRelationship from "@/components/ui/Relationships/CreateOrganisationRelationship.vue";
import { fmtToLocalShortDate } from "@/composable/date";
import { useEnum } from "@/composable/enum";
import {
  useInfiniteListable,
  useInfiniteTrigger
} from "@/composable/infinite-list";
import {
  OrganisationRelationshipType,
  OrganisationRelationships
} from "@/network/crm-service-proxies";
import { OrganisationStore } from "@/store/modules/organisations";
import { defineComponent, reactive, watch } from "vue";

export default defineComponent({
  props: {
    id: {
      type: [Number, String],
      required: true
    }
  },
  components: {
    Detail,
    AlpButton,
    showCreateRelationship
  },
  setup(props) {
    const state = reactive({
      showCreateRelationshipModal: false,
      search: ""
    });

    // const invoice = computed(() =>
    //   ApiStore.toData<InvoiceDto>(
    //     store.getters[ContactStore.getters.GET_CONTACT_RELATIONSHIPS](props.id)
    //   )
    // );

    const { items, loading, fetch, reset } = useInfiniteListable({
      items: OrganisationStore.getters.GET_ORGANISATION_RELATIONSHIPS,
      query: OrganisationStore.actions.GET_ORGANISATION_RELATIONSHIPS,

      queryParams: () => ({
        id: props.id,
        search: state.search
      })
    });

    watch([() => state.search], reset);

    function loadMore() {
      if (loading.value) {
        return;
      }

      fetch();
    }

    function closeCreate() {
      state.showCreateRelationshipModal = false;
      reset();
    }

    const { toDescription: toRelationshipType } = useEnum(
      OrganisationRelationshipType
    );
    const { toDescription: toRelationships } = useEnum(
      OrganisationRelationships
    );

    const { container, sentinel } = useInfiniteTrigger(loadMore);

    return {
      container,
      sentinel,
      state,
      items,
      loading,
      fmtToLocalShortDate,
      toRelationshipType,
      toRelationships,
      closeCreate
    };
  }
});
</script>
