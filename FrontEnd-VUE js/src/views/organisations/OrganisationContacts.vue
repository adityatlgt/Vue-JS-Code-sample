<template>
  <detail fill>
    <!-- <alp-heading heading="Contacts">
    </alp-heading> -->
    <div class="detail-inner-content overflow-y-auto p-2">
      <alp-divider>Contacts</alp-divider>
      <alp-infinite-table
        class="flex-1"
        :headers="['Full Name', 'Email', 'Contact No.']"
        :fields="['fullName', 'email', 'contactNumber']"
        :loading="organisation_contacts_loading"
        :values="organisation_contacts_items"
        @selected="$router.push({ name: 'Contact', params: { id: $event.id } })"
        @load-more="organisation_contacts_fetch"
        id="organisationContacts"
      >
      </alp-infinite-table>

      <alp-divider>Clients</alp-divider>
      <alp-infinite-table
        class="flex-1"
        :headers="['Client Name']"
        :fields="['clientName']"
        :loading="organisation_clients_loading"
        :values="organisation_clients_items"
        @selected="$router.push({ name: 'Client', params: { id: $event.id } })"
        @load-more="organisation_clients_fetch"
        id="organisationClients"
      >
      </alp-infinite-table>
    </div>
  </detail>
</template>

<script lang="ts">
import AlpButton from "@/components/common/AlpButton.vue";
import AlpHeading from "@/components/common/layout/AlpHeading.vue";
import Detail from "@/components/ui/layout/Detail.vue";
import showCreateRelationship from "@/components/ui/Relationships/CreateRelationship.vue";
import { fmtToLocalShortDate } from "@/composable/date";
import { useEnum } from "@/composable/enum";
import {
  useInfiniteListable,
  useInfiniteTrigger
} from "@/composable/infinite-list";
import { ContactStore } from "@/store/modules/contacts";
import { OrganisationStore } from "@/store/modules/organisations";
import { defineComponent, reactive, ref, watch } from "vue";

export default defineComponent({
  props: {
    id: {
      type: [Number, String],
      required: true
    }
  },
  components: {
    Detail
    // AlpButton,
    // showCreateRelationship,
    // AlpHeading
  },
  setup(props) {
    const state = reactive({
      // showCreateRelationshipModal: false,
      // search: ""
    });

    var OrganisationContactsTable = useInfiniteListable({
      items: ContactStore.getters.GET_ORGANISATIONS_CONTACTS,
      query: ContactStore.actions.GET_ORGANISATIONS_CONTACTS,

      queryParams: () => ({
        id: props.id
      })
    });

    var OrganisationClientsTable = useInfiniteListable({
      items: OrganisationStore.getters.GET_ORGANISATIONS_CLIENTS,
      query: OrganisationStore.actions.GET_ORGANISATIONS_CLIENTS,

      queryParams: () => ({
        id: props.id
      })
    });

    var organisation_contacts_items = OrganisationContactsTable.items;
    var organisation_contacts_loading = OrganisationContactsTable.loading;
    var organisation_contacts_fetch = OrganisationContactsTable.fetch;

    var organisation_clients_items = OrganisationClientsTable.items;
    var organisation_clients_loading = OrganisationClientsTable.loading;
    var organisation_clients_fetch = OrganisationClientsTable.fetch;

    // watch([() => state.search], reset);

    // function loadMore() {
    //   if (organisation_contacts_loading.value) {
    //     return;
    //   }
    //   organisation_contacts_fetch();
    // }

    // const { container, sentinel } = useInfiniteTrigger(loadMore);

    return {
      // container,
      // sentinel,
      state,
      fmtToLocalShortDate,
      OrganisationContactsTable,
      organisation_contacts_items,
      organisation_contacts_loading,
      organisation_contacts_fetch,

      OrganisationClientsTable,
      organisation_clients_items,
      organisation_clients_loading,
      organisation_clients_fetch
    };
  }
});
</script>
