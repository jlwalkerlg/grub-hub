import React, {
  FC,
  FormEvent,
  useState,
  useMemo,
  SyntheticEvent,
  useEffect,
  MouseEvent,
  useRef,
} from "react";
import debounce from "lodash/debounce";

import { useFormComponent } from "~/lib/Form/useFormComponent";
import {
  RequiredRule,
  PasswordRule,
  EmailRule,
  PhoneRule,
  PostCodeRule,
} from "~/lib/Form/Rule";
import { CompositeForm, Form } from "~/lib/Form/Form";
import addressSearcher, {
  AddressSearchResult,
} from "~/services/AddressSearch/AddressSearcher";
import RegisterRestaurantForm from "./RegisterRestaurantForm";

const RegisterRestaurantFormController: FC = () => {
  const managerName = useFormComponent("Jordan Walker", [new RequiredRule()]);
  const managerEmail = useFormComponent("jordan@walker.com", [
    new RequiredRule(),
    new EmailRule(),
  ]);
  const managerPassword = useFormComponent("password123", [
    new RequiredRule(),
    new PasswordRule(),
  ]);
  const restaurantName = useFormComponent("Chow Main", [new RequiredRule()]);
  const restaurantPhone = useFormComponent("01274 788944", [
    new RequiredRule(),
    new PhoneRule(),
  ]);
  const addressLine1 = useFormComponent("", [new RequiredRule()]);
  const addressLine2 = useFormComponent("");
  const city = useFormComponent("", [new RequiredRule()]);
  const postCode = useFormComponent("", [
    new RequiredRule(),
    new PostCodeRule(),
  ]);

  const [manual, setManual] = useState(false);

  const [addressSearchResults, setAddressSearchResults] = useState<
    AddressSearchResult[]
  >([]);

  const searchAddress = useRef(
    debounce((query: string) => {
      addressSearcher.search(query).then(setAddressSearchResults);
    }, 500)
  );

  useEffect(() => {
    if (addressLine1.value === "") {
      setAddressSearchResults([]);
      return;
    }

    if (manual) {
      setManual(false);
      return;
    }

    searchAddress.current(addressLine1.value);
  }, [addressLine1.value]);

  function onSelectAddress(e: MouseEvent<HTMLButtonElement>): void {
    e.preventDefault();

    setManual(true);
    setAddressSearchResults([]);

    const placeId = e.currentTarget.dataset.id;

    addressSearcher.getAddress(placeId).then((address) => {
      addressLine1.setValue(address.addressLine1);
      addressLine2.setValue(address.addressLine2);
      city.setValue(address.city);
      postCode.setValue(address.postCode);
    });
  }

  const [step, setStep] = useState(3);

  const form = useMemo(
    () =>
      new CompositeForm([
        new Form({ managerName, managerEmail, managerPassword }),
        new Form({ restaurantName, restaurantPhone }),
        new Form({ addressLine1, addressLine2, city, postCode }),
      ]),
    [
      managerName,
      managerEmail,
      managerPassword,
      restaurantName,
      restaurantPhone,
      addressLine1,
      addressLine2,
      city,
      postCode,
    ]
  );

  const [canAdvance, setCanAdvance] = useState(() => form.validateForm(0));

  useEffect(() => {
    setCanAdvance(form.validateForm(step - 1));
  }, [form]);

  function advanceStep(e: SyntheticEvent) {
    e.preventDefault();

    if (canAdvance) {
      setStep(step + 1);
    }
  }

  function backStep() {
    setStep(step - 1);
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault();

    console.log("values", form.values);
  }

  return (
    <RegisterRestaurantForm
      addressSearchResults={addressSearchResults}
      onSelectAddress={onSelectAddress}
      managerName={managerName}
      managerEmail={managerEmail}
      managerPassword={managerPassword}
      restaurantName={restaurantName}
      restaurantPhone={restaurantPhone}
      addressLine1={addressLine1}
      addressLine2={addressLine2}
      city={city}
      postCode={postCode}
      step={step}
      canAdvance={canAdvance}
      advanceStep={advanceStep}
      backStep={backStep}
      onSubmit={onSubmit}
    />
  );
};

export default RegisterRestaurantFormController;
