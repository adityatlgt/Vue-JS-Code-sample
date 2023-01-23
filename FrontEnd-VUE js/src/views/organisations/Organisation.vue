<template>
  <master-detail
    :backLink="{ link: { name: 'Organisations' }, name: 'Organisations' }"
    :links="availableOrganisationLinks"
  >
    <!-- <span
      class="flex items-center justify-center mt-3 w-56 h-32 self-center bg-white"
    >
      <img class="w-24 my-6" src="/img/logo/andreyev-logo.png" alt="logo" />
    </span> -->

    <div class="flex items-center text-sm mx-3 flex-1 py-1 px-1 leading-8">
      <div class="w-full">
        <span class="text-2xl font-semibold">
          {{ organisation?.name }}
        </span>
      </div>

      <!-- <template v-if="organisation?.website">
        <span class="master-info-title">Website</span>
        <a :href="processedWebsite" target="_blank">
          <span class="master-info hover:underline">
            {{ organisation?.website }}
          </span>
        </a>
      </template> -->
    </div>
  </master-detail>
</template>

<script lang="ts">
import MasterDetail from "@/components/ui/layout/MasterDetail.vue";
import { OrganisationDto } from "@/network/organisation-service-proxies";
import { OrganisationStore } from "@/store/modules/organisations";
import { ApiStore } from "@/store/utils";
import { computed, defineComponent, onMounted } from "vue";
import { useStore } from "vuex";

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  components: {
    MasterDetail
  },
  setup(props) {
    const store = useStore();
    const organisation = computed(() =>
      ApiStore.toData<OrganisationDto>(
        store.getters[OrganisationStore.getters.GET_ORGANISATION](props.id)
      )
    );

    const processedWebsite = computed(() => {
      const website = organisation.value?.website;
      if (website && !/^(?:f|ht)tps?:\/\//.test(website)) {
        return "https://" + website;
      }
      return website;
    });

    function fetchOrganisation() {
      store.dispatch(OrganisationStore.actions.GET_ORGANISATION, {
        id: props.id
      });
    }

    const availableOrganisationLinks = computed(() => {
      const baseLinks = [
        {
          link: { name: "Organisation Info" },
          name: "Info",
          icon: "fa-solid fa-circle-info"
        },
        {
          link: { name: "Organisation Notes" },
          name: "Notes",
          permissions: ["OrganisationNote.View"],
          icon: "fa-solid fa-message"
        },

        {
          link: { name: "Organisation Safe Storage Documents" },
          name: "Safe Storage",
          icon: "fa-solid fa-box-archive"
        },

        {
          link: { name: "Organisation Metadata" },
          name: "Metadata",
          permissions: ["OrganisationMetadata.View"],
          icon: "fa-solid fa-tags"
        },
        {
          link: { name: "Organisation Documents" },
          name: "Documents",
          permissions: ["OrganisationDocument.View"],
          icon: "fa-solid fa-file-lines"
        },
        {
          link: { name: "Organisation Contacts" },
          name: "Contacts / Clients",
          icon: "fa-solid fa-address-book"
        },
        {
          link: { name: "Organisation Relationships" },
          name: "Relationships",
          icon: "fa-solid fa-city"
        }
      ];

      if (organisation.value?.showEmails) {
        return [
          ...baseLinks,
          {
            link: { name: "Organisation Emails" },
            name: "Emails",
            permissions: ["OrganisationEmail.View"],
            icon: "fa-solid fa-envelope"
          }
        ];
      }

      return baseLinks;
    });

    onMounted(fetchOrganisation);

    return { organisation, processedWebsite, availableOrganisationLinks };
  }
});
</script>
