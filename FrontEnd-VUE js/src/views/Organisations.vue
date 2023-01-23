<template>
  <alp-container class="bg-white">
    <alp-heading heading="Organisations">
      <alp-can permission="Organisation.Create">
        <alp-button
          class="text-xs addOrganisation"
          @click="showCreateOrganisationModal"
          variant="primary"
        >
          <alp-icon icon="fa-solid fa-plus" class="mr-2" size="14" />
          <span>Add Organisation</span>
        </alp-button>
      </alp-can>
    </alp-heading>

    <div class="inner-content flex flex-col overflow-y-hidden p-2">
      <alp-focus-input
        class="text-sm rounded-md py-2 w-full"
        placeholder="Search (Press 'Ctrl + /' to focus)"
        v-model="state.search"
      />

      <alp-infinite-table
        class="flex-1 mt-2"
        :headers="['Name', 'Website', 'Status']"
        :fields="['name', 'website', 'status']"
        :loading="loading"
        :values="items"
        @selected="$router.push({ name: 'Organisation', params: $event })"
        @load-more="fetch"
      >
        <template v-slot:status="{ value }">
          <td v-if="value.status == 1">Active</td>
          <td v-if="value.status == 2">Dormant</td>
          <td v-if="value.status == 3">Inactive</td>
          <td v-if="value.status == 4">Inactive</td>
        </template>
      </alp-infinite-table>
    </div>
  </alp-container>
</template>

<script lang="ts">
import AlpContainer from "@/components/common/layout/AlpContainer.vue";
import { useInfiniteListable } from "@/composable/infinite-list";
import { ModalStore, ModalType } from "@/store/modules/modals";
import { OrganisationStore } from "@/store/modules/organisations";
import { defineComponent, reactive, watch } from "vue";
import { useStore } from "vuex";
import { enumFilterState, FilterState } from "@/store/filterState";

export default defineComponent({
  components: { AlpContainer },
  setup() {
    let filterState = new FilterState();
    const filterStateData = GetFilterState();

    function GetFilterState() {
      let filterData = filterState.getItem(enumFilterState.UserOrganisations);
      if (filterData) return filterData;
      return {
        search: ""
      };
    }

    const store = useStore();

    function showCreateOrganisationModal() {
      store.dispatch(ModalStore.actions.SHOW_MODAL, {
        modal: ModalType.CreateOrganisation
      });
    }

    const state = reactive({
      search: filterStateData.search
    });

    const { items, loading, fetch, reset } = useInfiniteListable({
      items: OrganisationStore.getters.GET_ORGANISATIONS,
      query: OrganisationStore.actions.GET_ORGANISATIONS,
      queryParams: () => ({
        search: state.search
      })
    });

    function SetFilterState() {
      let filterStateData = {
        search: state.search
      };
      filterState.setItem(enumFilterState.UserOrganisations, filterStateData);
    }

    watch([() => state.search], () => {
      SetFilterState();
      reset();
    });

    return {
      state,
      items,
      loading,
      fetch,
      showCreateOrganisationModal
    };
  }
});
</script>
