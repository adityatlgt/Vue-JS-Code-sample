<template>
  <alp-container class="bg-white">
    <!-- <alp-heading heading="Safe Storage Documents"> -->

    <div class="inner-content flex flex-col overflow-y-auto p-1">
      <span class="flex justify-between pb-3">
        <alp-focus-input
          class="flex-1 flex text-sm"
          placeholder="Search (Press 'Ctrl + /' to focus)"
          v-model="state.search"
        />
        <alp-can permission="CommonType.Create">
          <alp-button
            class="text-xs ml-3"
            @click="state.showCreate = true"
            variant="primary"
            id="safeStorageRecord"
          >
            <alp-icon icon="fa-solid fa-plus" class="mr-2" size="14" />
            <span>Add Safe Storage Record</span>
          </alp-button>
        </alp-can>
      </span>

      <alp-infinite-table
        class="flex-1"
        :headers="[
          'Storage Code',
          'Office',
          'Document Type',
          'Status',
          ' Date'
        ]"
        :fields="[
          'safeStorageSection.storageCode',
          'safeStorageSection.office.legalEntity',
          'safeStorageDocumentType.name',
          'status',
          'dateOfDocuments'
        ]"
        :loading="loading"
        :values="items"
        @selected="
          $router.push({
            name: 'Organisation Safe Storage Document',
            params: { documentId: $event.id }
          })
        "
        @load-more="fetch"
      >
        <template v-slot:dateOfDocuments="{ value }">
          <td>
            {{ fmtToLocalShortDate(value.dateOfDocuments) }}
          </td>
        </template>
        <template v-slot:nominatedType="{ value }">
          <td>
            {{ enum_nominatedType(value.nominatedType) }}
          </td>
        </template>
        <template v-slot:status="{ value }">
          <td>
            {{ enum_status(value.status) }}
          </td>
        </template>
      </alp-infinite-table>
    </div>

    <create-organisation-safe-storage
      v-if="state.showCreate"
      :organisation_id="id"
      @close="state.showCreate = false"
    />

    <router-view v-slot="{ Component }">
      <transition name="slide-over" mode="out-in">
        <component
          :is="Component"
          :key="$route.params.id"
          @close="$router.push({ name: 'Organisation Safe Storage Documents' })"
        />
      </transition>
    </router-view>
  </alp-container>
</template>

<script lang="ts">
import { useInfiniteListable } from "@/composable/infinite-list";
import { CommonStore } from "@/store/modules/common";
import { defineComponent, reactive, watch } from "vue";
import { fmtToLocalShortDate } from "@/composable/date";
import CreateOrganisationSafeStorage from "@/components/ui/safestorage/CreateOrganisationSafeStorage.vue";

export default defineComponent({
  props: {
    id: {
      type: [String, Number],
      required: true
    }
  },
  components: {
    CreateOrganisationSafeStorage
  },
  setup(props) {
    const state = reactive({
      showCreate: false,
      search: ""
    });

    const { items, loading, fetch, reset } = useInfiniteListable({
      items: CommonStore.getters.GET_ORGANISATION_SAFE_STORAGE_DOCUMENTS,
      query: CommonStore.actions.GET_ORGANISATION_SAFE_STORAGE_DOCUMENTS,
      queryParams: () => ({
        id: props.id,
        search: state.search
      })
    });

    enum NominatedTypes {
      Executor = 1,
      Attorney = 2,
      Guardian = 3,
      Donee = 4,
      SubstituteDecisionMaker = 5,
      Beneficiary = 6,
      Other = 7,
      NoSelection = 8
    }

    enum SafeStorageStatusTypes {
      OriginalHeldByAndreyevLawyers = 1,
      Removed = 2,
      NotReturned = 3,
      TransitMoneyHeldByAndreyevLawyers = 4,
      Scanned = 5
    }
    function enum_nominatedType(input: any) {
      var result;
      switch (input) {
        case NominatedTypes.Executor: {
          result = "Executor";
          break;
        }
        case NominatedTypes.Attorney: {
          result = "Attorney";
          break;
        }
        case NominatedTypes.Guardian: {
          result = "Guardian";
          break;
        }
        case NominatedTypes.Donee: {
          result = "Donee";
          break;
        }
        case NominatedTypes.SubstituteDecisionMaker: {
          result = "SubstituteDecisionMaker";
          break;
        }
        case NominatedTypes.Beneficiary: {
          result = "Beneficiary";
          break;
        }
        case NominatedTypes.Other: {
          result = "Other";
          break;
        }
        case NominatedTypes.NoSelection: {
          result = "NoSelection";
          break;
        }
        default: {
          result = "";
          break;
        }
      }
      return result;
    }

    function enum_status(input: any) {
      var result;
      switch (input) {
        case SafeStorageStatusTypes.OriginalHeldByAndreyevLawyers: {
          result = "OriginalHeldByAndreyevLawyers";
          break;
        }
        case SafeStorageStatusTypes.Removed: {
          result = "Removed";
          break;
        }
        case SafeStorageStatusTypes.NotReturned: {
          result = "NotReturned";
          break;
        }
        case SafeStorageStatusTypes.TransitMoneyHeldByAndreyevLawyers: {
          result = "TransitMoneyHeldByAndreyevLawyers";
          break;
        }
        case SafeStorageStatusTypes.Scanned: {
          result = "Scanned";
          break;
        }
        default: {
          result = "";
          break;
        }
      }
      return result;
    }

    watch([() => state.search], reset);

    return {
      state,
      items,
      loading,
      fetch,
      fmtToLocalShortDate,
      NominatedTypes,
      enum_nominatedType,
      enum_status,
      SafeStorageStatusTypes
    };
  }
});
</script>
