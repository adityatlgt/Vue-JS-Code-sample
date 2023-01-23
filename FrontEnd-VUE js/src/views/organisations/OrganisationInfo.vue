<template>
  <detail fill>
    <!-- <alp-heading heading="Info" /> -->
    <div class="detail-inner-content overflow-y-auto">
      <alp-section>
        <alp-form-divider name="General Information" />
        <alp-form-container>
          <field-label class="w-full" name="Name">
            <inline-input
              placeholder="Name"
              v-model="state.name"
              rules="required"
            />
          </field-label>
          <field-label class="w-full md:w-1/2" name="Website">
            <inline-input
              placeholder="Website"
              v-model="state.website"
              rules="url"
            />
          </field-label>

          <field-label class="w-full md:w-1/2" name="Main Line">
            <inline-input
              placeholder="Main Line"
              v-model="state.mainline"
              rules="phone"
            />
          </field-label>

          <field-label class="w-full md:w-1/2" name="Establishment Date">
            <date-input
              placeholder="Establishment Date"
              v-model="state.establishmentDate"
            />
          </field-label>

          <label class="form-checkbox-field w-1/2 flex py-8">
            <input
              type="checkbox"
              v-model="state.establishedByUs"
            />
            <span class="form-label mr-2">Established By Us</span>
          </label>

          <field-label class="w-full md:w-1/2" name="Estimated Revenue*">
            <select v-model="state.estimatedRevenue">
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
            </select>
          </field-label>

          <field-label
            class="w-full md:w-1/2"
            name="Estimated Number of Employees*"
          >
            <select v-model="state.numberOfEmployees">
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
            </select>
          </field-label>

          <field-label class="w-full md:w-1/2" name="Business Lifecycle">
            <select v-model="state.businessLifecycle">
              <option value="" disabled>Business Lifecycle</option>
              <option :value="BusinessLifecycleType.Launch">Launch</option>
              <option :value="BusinessLifecycleType.Growth">Growth</option>
              <option :value="BusinessLifecycleType.Mature">Mature</option>
              <option :value="BusinessLifecycleType.Decline">Decline</option>
            </select>
          </field-label>

          <field-label class="w-full md:w-1/2" name="Number of Locations">
            <select v-model="state.numberOfLocations">
              <option value="" disabled>Number of Locations</option>
              <option :value="NumberOfLocationsType.One">1</option>
              <option :value="NumberOfLocationsType.Two">2</option>
              <option :value="NumberOfLocationsType.GreaterThanTwo">
                &gt; 2
              </option>
            </select>
          </field-label>

          <field-label class="w-full md:w-1/2" name="Industry Category">
            <industry-category-selector v-model="state.industryCategory" />
          </field-label>

          <field-label class="w-full md:w-1/2" name="Industry Sub Category">
            <industry-sub-category-selector
              v-model="state.industrySubCategory"
            />
          </field-label>

          <field-label class="w-full md:w-1/2" name="Organisation Type">
            <organisation-type-selector v-model="state.organisationType" />
          </field-label>

          <field-label class="w-full md:w-1/2" name="ABN">
            <inline-input placeholder="ABN" v-model="state.abn" type="number" />
          </field-label>

          <field-label
            v-if="state.organisationType?.hasAcn"
            class="w-full md:w-1/2"
            name="ACN"
          >
            <inline-input placeholder="ACN" v-model="state.acn" />
          </field-label>
        </alp-form-container>
      </alp-section>

      <alp-section v-if="state.abn">
        <alp-form-divider name="ABN Lookup (External Data)" />
        <alp-form-container>
          <field-label class="flex w-1/2" name="ABN">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.Abn)">
              <span class="flex"> {{abnlookup.Abn}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>

          <field-label class="w-1/2" name="ACN">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.Acn)">
              <span class="flex"> {{abnlookup.Acn}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>

          <field-label class="w-1/2" name="Abn Status">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.AbnStatus)">
              <span class="flex"> {{abnlookup.AbnStatus}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Abn Status Effective From">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.AbnStatusEffectiveFrom)">
              <span class="flex"> {{abnlookup.AbnStatusEffectiveFrom}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>

          </field-label>
          <field-label class="w-1/2" name="Address Date">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.AddressDate)">
              <span class="flex"> {{abnlookup.AddressDate}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Address State">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.AddressState)">
              <span class="flex"> {{abnlookup.AddressState}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Business Name">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.BusinessName)">
              <span class="flex"> {{abnlookup.BusinessName}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Entity Name">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.EntityName)">
              <span class="flex"> {{abnlookup.EntityName}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Entity Type Code">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.EntityTypeCode)">
              <span class="flex"> {{abnlookup.EntityTypeCode}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Entity Type Name">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.EntityTypeName)">
              <span class="flex"> {{abnlookup.EntityTypeName}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Gst">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.Gst)">
              <span class="flex"> {{abnlookup.Gst}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>
          <field-label class="w-1/2" name="Message">
            <span class="pl-1 flex flex-wrap text-sm" @click="copyValue(abnlookup.Message)">
              <span class="flex"> {{abnlookup.Message}} </span>
              <alp-icon icon="feather:clipboard" class="ml-2" size="18" />
            </span>
          </field-label>

          <!-- <label class="form-checkbox-field w-1/2 py-8">
            <input
              class="checkbox"
              type="checkbox"
              v-model="state.showEmails"
            />
            <span class="form-label mr-2">Show Emails</span>
          </label> -->
        </alp-form-container>
      </alp-section>

      <alp-section>
        <alp-form-divider name="Address" />
        <alp-form-container v-if="state.address">
          <field-label class="w-full" name="Address Lookup">
            <google-places-autocomplete
              class="w-full googleAutoComplete"
              placeholder="Address Lookup"
              @selected="setAddress(state.address, $event)"
            />
          </field-label>

          <field-label class="w-full md:w-1/2" name="Address">
            <inline-input
              placeholder="Address"
              v-model="state.address.address1"
            />
          </field-label>

          <field-label class="w-full md:w-1/2" name="Address 2">
            <inline-input
              placeholder="Address 2"
              v-model="state.address.address2"
            />
          </field-label>

          <field-label class="w-1/2 md:w-1/3" name="Suburb">
            <inline-input placeholder="Suburb" v-model="state.address.suburb" />
          </field-label>

          <field-label class="w-1/2 md:w-1/6" name="State*">
            <inline-input
              placeholder="State"
              v-model="state.address.state"
              rules="required"
            />
          </field-label>

          <field-label class="w-1/2 md:w-1/6" name="Postcode">
            <inline-input
              placeholder="Postcode"
              v-model="state.address.postcode"
            />
          </field-label>

          <field-label class="w-1/2 md:w-1/3" name="Country*">
            <inline-input
              placeholder="Country"
              v-model="state.address.country"
              rules="required"
            />
          </field-label>
        </alp-form-container>
      </alp-section>

      <alp-section>
        <alp-form-divider name="Postal Address" />

        <label class="form-checkbox-field w-4/12">
          <input
            name="postalSameAsAddress"
            type="checkbox"
            v-model="state.postalSameAsAddress"
          />
          <span class="form-label mr-2">Same as address</span>
        </label>

        <alp-form-container v-if="state.postalAddress">
          <field-label
            class="w-full"
            name="Address Lookup"
            v-show="!state.postalSameAsAddress"
          >
            <google-places-autocomplete
              class="w-full"
              placeholder="Address Lookup"
              @selected="setAddress(state.postalAddress, $event)"
            />
          </field-label>

          <field-label
            class="w-full md:w-1/2"
            name="Address"
            v-show="!state.postalSameAsAddress"
          >
            <inline-input
              placeholder="Address"
              v-model="state.postalAddress.address1"
            />
          </field-label>

          <field-label
            class="w-full md:w-1/2"
            name="Address 2"
            v-show="!state.postalSameAsAddress"
          >
            <inline-input
              placeholder="Address 2"
              v-model="state.postalAddress.address2"
            />
          </field-label>

          <field-label
            class="w-1/2 md:w-1/3"
            name="Suburb"
            v-show="!state.postalSameAsAddress"
          >
            <inline-input
              placeholder="Suburb"
              v-model="state.postalAddress.suburb"
            />
          </field-label>

          <field-label
            class="w-1/2 md:w-1/6"
            name="State"
            v-show="!state.postalSameAsAddress"
          >
            <inline-input
              placeholder="State"
              v-model="state.postalAddress.state"
              rules="required"
            />
          </field-label>

          <field-label
            class="w-1/2 md:w-1/6"
            name="Postcode"
            v-show="!state.postalSameAsAddress"
          >
            <inline-input
              placeholder="Postcode"
              v-model="state.postalAddress.postcode"
            />
          </field-label>

          <field-label
            class="w-1/2 md:w-1/3"
            name="Country"
            v-show="!state.postalSameAsAddress"
          >
            <inline-input
              placeholder="Country"
              v-model="state.postalAddress.country"
              rules="required"
            />
          </field-label>
        </alp-form-container>
      </alp-section>

      <alp-can permission="Email.ManagePermissions">
        <alp-section>
          <alp-form-divider name="Internal" />
          <alp-form-container>
            <field-label class="w-1/2" name="Status">
              <select v-model="state.status">
                <option
                  v-for="option in status"
                  :key="option.value"
                  :value="option.value"
                >
                  {{ option.key }}
                </option>
              </select>
            </field-label>

            <label class="form-checkbox-field w-1/2 py-8">
              <input
                type="checkbox"
                v-model="state.showEmails"
              />
              <span class="form-label mr-2">Show Emails</span>
            </label>
          </alp-form-container>
        </alp-section>
      </alp-can>
    </div>
  </detail>
</template>

<script lang="ts">
import FieldLabel from "@/components/forms/FieldLabel.vue";
import GooglePlacesAutocomplete from "@/components/inputs/GooglePlacesAutocomplete.vue";
import InlineInput from "@/components/inputs/InlineInput.vue";
import DateInput from "@/components/inputs/DateInput.vue";
import Detail from "@/components/ui/layout/Detail.vue";
import { usePatchable } from "@/composable/patchable";
import OrganisationTypeSelector from "@/components/inputs/selectors/OrganisationTypeSelector.vue";
import IndustryCategorySelector from "@/components/inputs/selectors/IndustryCategorySelector.vue";
import IndustrySubCategorySelector from "@/components/inputs/selectors/IndustrySubCategorySelector.vue";
import {
  EstimatedRevenueType,
  NumberOfEmployeesType,
  BusinessLifecycleType,
  NumberOfLocationsType
} from "@/network/crm-service-proxies";
import { OrganisationStore } from "@/store/modules/organisations";
import { defineComponent, onBeforeMount, onMounted, reactive } from "vue";
import { useEnum } from "@/composable/enum";
import {
  OrganisationDto,
  OrganisationsStatusType
} from "@/network/organisation-service-proxies";
import store from "@/store";
import { Field as VField } from "vee-validate";
import { useClipboard } from "@vueuse/core";
import { useNotify } from "@/composable/notify";

export default defineComponent({
  props: {
    id: {
      type: String,
      required: true
    }
  },
  components: {
    DateInput,
    InlineInput,
    Detail,
    VField,
    FieldLabel,
    GooglePlacesAutocomplete,
    OrganisationTypeSelector,
    IndustryCategorySelector,
    IndustrySubCategorySelector
  },
  setup(props) {
    const { toDropdownOptions: toStatusOptions } = useEnum(
      OrganisationsStatusType
    );
    const status = toStatusOptions();
    const { copy } = useClipboard();
    const { fireSuccessToast } = useNotify();

    const { state } = usePatchable<OrganisationDto>({
      identifier: props.id,
      getter: OrganisationStore.getters.GET_ORGANISATION,
      query: OrganisationStore.actions.GET_ORGANISATION,
      queryParams: () => ({ id: props.id }),
      patchQuery: OrganisationStore.actions.PATCH_ORGANISATION,
      patchQueryParams: () => ({
        id: props.id
      })
    });

    const abnlookup = reactive({
      Abn: "",
      AbnStatus: "",
      AbnStatusEffectiveFrom: "",
      Acn: "",
      AddressDate: "",
      AddressPostcode: "",
      AddressState: "",
      BusinessName: "",
      EntityName: "",
      EntityTypeCode: "",
      EntityTypeName: "",
      Gst: "",
      Message: ""
    });

    async function getAbnLookUpData() {
      return await store
        .dispatch(OrganisationStore.getters.GET_ORGANISATION_ABN_DATA, {
          id: props.id
        })
        .then(
          (values) => (
            (abnlookup.Abn = values?.Abn),
            (abnlookup.AbnStatus = values?.AbnStatus),
            (abnlookup.AbnStatusEffectiveFrom = values?.AbnStatusEffectiveFrom),
            (abnlookup.Acn = values?.Acn),
            (abnlookup.AddressDate = values?.AddressDate),
            (abnlookup.AddressPostcode = values?.AddressPostcode),
            (abnlookup.AddressState = values?.AddressState),
            (abnlookup.BusinessName = values?.BusinessName),
            (abnlookup.EntityName = values?.EntityName),
            (abnlookup.EntityTypeCode = values?.EntityTypeCode),
            (abnlookup.EntityTypeName = values?.EntityTypeName),
            (abnlookup.Gst = values?.Gst),
            (abnlookup.Message = values?.Message)
          )
        );
    }

    function setAddress(values: any, address: any) {
      values.address1 = address.address;
      values.address2 = "";
      values.suburb = address.suburb;
      values.state = address.state;
      values.country = address.country;
      values.postcode = address.postalCode;
    }


    function copyValue(value: any){
      copy(value).then(() =>
          fireSuccessToast("Value copied to clipboard !")
          );
    }





    onMounted(async () => await getAbnLookUpData());

    return {
      state,
      EstimatedRevenueType,
      NumberOfEmployeesType,
      BusinessLifecycleType,
      NumberOfLocationsType,
      setAddress,
      status,
      abnlookup,
      getAbnLookUpData,
      copyValue
    };
  }
});
</script>
