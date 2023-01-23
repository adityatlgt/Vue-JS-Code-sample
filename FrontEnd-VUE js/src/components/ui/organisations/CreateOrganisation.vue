<template>
  <modal @close="$emit('close')">
    <modal-form
      @cancel="$emit('close')"
      @submit="createOrganisation"
      v-slot="{ values }"
      :initialValues="{
        dueDate: new Date().toISOString().substr(0, 10)
      }"
    >
      <alp-form-divider name="Create Organisation" />

      <alp-form-container>
        <field-label class="w-full" name="Name*">
          <v-field
            type="text"
            placeholder="Name"
            name="name"
            rules="required"
          />
          <error-message class="error-message" name="name" />
        </field-label>

        <field-label class="w-full sm:w-1/2" name="Estimated Revenue*">
          <v-field
            as="select"
            name="estimatedRevenue"
            rules="required"
          >
            <option value="" disabled>Estimated Revenue</option>
            <option :value="EstimatedRevenueType.Micro">
              Micro ($0 - $2m)
            </option>
            <option :value="EstimatedRevenueType.Small">
              Small (&lt; $5m)
            </option>
            <option :value="EstimatedRevenueType.Medium">
              Medium ($5m - $50m)
            </option>
            <option :value="EstimatedRevenueType.Large">
              Large (&gt; $50m)
            </option>
          </v-field>

          <error-message class="error-message" name="estimatedRevenue" />
        </field-label>

        <field-label
          class="w-full sm:w-1/2"
          name="Estimated Number of Employees*"
        >
          <span class="flex flex-col text-xs">
            <v-field
              as="select"
              name="numberOfEmployees"
              rules="required"
            >
              <option value="" disabled>Estimated Number of Employees</option>
              <option :value="NumberOfEmployeesType.Micro">
                Micro (&lt; 5)
              </option>
              <option :value="NumberOfEmployeesType.Small">
                Small (5 - 19)
              </option>
              <option :value="NumberOfEmployeesType.Medium">
                Medium (20 - 199)
              </option>
              <option :value="NumberOfEmployeesType.Large">
                Large (&gt; 200)
              </option>
            </v-field>
          </span>
          <error-message class="error-message" name="numberOfEmployees" />
        </field-label>

        <field-label class="w-full sm:w-1/2" name="Organisation Type*">
          <organisation-type-selector-field name="organisationType" class="organisationType" />
          <error-message class="error-message" name="organisationType" />
        </field-label>

        <field-label class="w-full sm:w-1/2" name="Industry Category*">
          <industry-category-selector-field
            name="industryCategory"
            @selected="() => (values.industrySubCategory = null)"
          />
          <error-message class="error-message" name="industryCategory" />
        </field-label>

        <field-label
          v-if="values.industryCategory"
          class="w-full sm:w-1/2"
          name="Industry Sub Category*"
        >
          <industry-sub-category-selector-field
            name="industrySubCategory"
            :industry-category-id="values.industryCategory?.id"
          />
          <error-message class="error-message" name="industrySubCategory" />
        </field-label>
      </alp-form-container>

      <alp-form-divider name="Address*" />

      <alp-form-container>
        <field-label class="w-full" name="Address Lookup">
          <google-places-autocomplete
            class="w-full googleAutolookup"
            placeholder="Address Lookup"
            @selected="setAddress(values.address, $event)"
          />
        </field-label>
        <field-label class="w-full sm:w-1/2" name="Address">
          <v-field type="text" placeholder="Address" name="address.address1" />
          <error-message class="error-message" name="address1" />
        </field-label>

        <field-label class="w-full sm:w-1/2" name="Address 2">
          <v-field
            type="text"
            placeholder="Address 2"
            name="address.address2"
          />
          <error-message class="error-message" name="address2" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/3" name="Suburb">
          <v-field type="text" placeholder="Suburb" name="address.suburb" />
          <error-message class="error-message" name="suburb" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/6" name="State">
          <v-field
            type="text"
            placeholder="State"
            name="address.state"
            rules="required"
          />
          <error-message class="error-message" name="address.state" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/6" name="Postcode">
          <v-field type="text" placeholder="Postcode" name="address.postcode" />
          <error-message class="error-message" name="postcode" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/3" name="Country">
          <v-field
            type="text"
            placeholder="Country"
            name="address.country"
            rules="required"
          />
          <error-message class="error-message" name="address.country" />
        </field-label>
      </alp-form-container>

      <!--Postal Address -->

      <alp-form-divider name="Postal Address" />

      <alp-form-container>
        <label class="form-checkbox-field w-full sm:w-3/12">
          <input name="postalSameAsAddress" type="checkbox" @click="changeisSameAddressChecked" checked="true" />
          <span class="form-label">Same as address</span>
        </label>
      </alp-form-container>

      <alp-form-container v-if="!state.checked">
        <field-label class="w-full" name="Address Lookup">
          <google-places-autocomplete
            class="w-full"
            placeholder="Address Lookup"
            @selected="setAddress(values.postalAddress, $event)"
          />
        </field-label>

        <field-label class="w-full sm:w-1/2" name="Address">
          <v-field
            type="text"
            :class="{ 'form-input-disabled': values.postalSameAsAddress }"
            placeholder="Address"
            name="postalAddress.address1"
            :disabled="values.postalSameAsAddress"
          />
          <error-message class="error-message" name="postalAddress.address1" />
        </field-label>

        <field-label class="w-full sm:w-1/2" name="Address 2">
          <v-field
            type="text"
            :class="{ 'form-input-disabled': values.postalSameAsAddress }"
            placeholder="Address 2"
            name="postalAddress.address2"
            :disabled="values.postalSameAsAddress"
          />
          <error-message class="error-message" name="postalAddress.address2" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/3" name="Suburb">
          <v-field
            type="text"
            :class="{ 'form-input-disabled': values.postalSameAsAddress }"
            placeholder="Suburb"
            name="postalAddress.suburb"
            :disabled="values.postalSameAsAddress"
          />
          <error-message class="error-message" name="postalAddress.suburb" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/6" name="State">
          <v-field
            type="text"
            :class="{ 'form-input-disabled': values.postalSameAsAddress }"
            placeholder="State"
            name="postalAddress.state"
            :disabled="values.postalSameAsAddress"
            :rules="values.postalSameAsAddress ? '' : 'required'"
          />
          <error-message class="error-message" name="postalAddress.state" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/6" name="Postcode">
          <v-field
            type="text"
            :class="{ 'form-input-disabled': values.postalSameAsAddress }"
            placeholder="Postcode"
            name="postalAddress.postcode"
            :disabled="values.postalSameAsAddress"
          />
          <error-message class="error-message" name="postalAddress.postcode" />
        </field-label>

        <field-label class="w-1/2 sm:w-1/3" name="Country">
          <v-field
            type="text"
            :class="{ 'form-input-disabled': values.postalSameAsAddress }"
            placeholder="Country"
            name="postalAddress.country"
            :disabled="values.postalSameAsAddress"
            :rules="values.postalSameAsAddress ? '' : 'required'"
          />
          <error-message class="error-message" name="postalAddress.country" />
        </field-label>
      </alp-form-container>
    </modal-form>
  </modal>
</template>

<script lang="ts">
import Modal from "@/components/common/layout/Modal.vue";
import ModalForm from "@/components/common/layout/ModalForm.vue";
import FieldLabel from "@/components/forms/FieldLabel.vue";
import GooglePlacesAutocomplete from "@/components/inputs/GooglePlacesAutocomplete.vue";
import OrganisationTypeSelectorField from "@/components/forms/selectors/OrganisationTypeSelectorField.vue";
import IndustryCategorySelectorField from "@/components/forms/selectors/IndustryCategorySelectorField.vue";
import IndustrySubCategorySelectorField from "@/components/forms/selectors/IndustrySubCategorySelectorField.vue";
import {
  IndustryCategoryDto,
  IndustryCategoryServiceProxy,
  IndustrySubCategoryDto,
  IndustrySubCategoryServiceProxy
} from "@/network/common-service-proxies";
import {
  EstimatedRevenueType,
  NumberOfEmployeesType
} from "@/network/crm-service-proxies";
import { OrganisationStore } from "@/store/modules/organisations";
import { ErrorMessage, Field as VField } from "vee-validate";
import { defineComponent, onMounted, reactive } from "vue";
import { useStore } from "vuex";


export default defineComponent({
  props: {},
  emits: ["created", "close"],
  components: {
    Modal,
    ModalForm,
    VField,
    FieldLabel,
    ErrorMessage,
    GooglePlacesAutocomplete,
    OrganisationTypeSelectorField,
    IndustryCategorySelectorField,
    IndustrySubCategorySelectorField
  },
  setup(props, { emit }) {
    const state = reactive({
      industryCategories: [] as IndustryCategoryDto[],
      industrySubCategories: [] as IndustrySubCategoryDto[],
      checked:true
    });

    function fetchIndustryCategories() {
      new IndustryCategoryServiceProxy()
        .getIndustryCategoryFullList()
        .then((result) => {
          state.industryCategories = result;
        });
    }

    function fetchSubIndustryCategories() {
      new IndustrySubCategoryServiceProxy()
        .getIndustrySubCategoryFullList()
        .then((result) => {
          state.industrySubCategories = result;
        });
    }

    onMounted(() => {
      fetchIndustryCategories();
      fetchSubIndustryCategories();
      
    });

    const store = useStore();


    let isSameAddressChecked = true;

    function changeisSameAddressChecked(){
      state.checked = !state.checked;
    }

    function createOrganisation(values: any) {
      values.postalSameAsAddress = state.checked;
      if (values.postalSameAsAddress) {
        values.postalAddress = undefined;
      }
      store
        .dispatch(OrganisationStore.actions.CREATE_ORGANISATION, {
          ...values,
          organisationTypeId: values.organisationType.id,
          industryCategoryId: values.industryCategory.id,
          industrySubCategoryId: values.industrySubCategory.id
        })
        .then((organisation) => {
          emit("created", organisation);
          emit("close");
        });
    }

    function setAddress(values: any, address: any) {
      values.address1 = address.address;
      values.address2 = "";
      values.suburb = address.suburb;
      values.state = address.state;
      values.country = address.country;
      values.postcode = address.postalCode;
    }

    return {
      state,
      changeisSameAddressChecked,
      isSameAddressChecked,
      createOrganisation,
      EstimatedRevenueType,
      NumberOfEmployeesType,
      setAddress
    };
  }
});
</script>
