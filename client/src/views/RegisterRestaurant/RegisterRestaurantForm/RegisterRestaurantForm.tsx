import React, { FC, FormEvent, SyntheticEvent, KeyboardEvent } from "react";

import { FormComponent } from "~/lib/Form/useFormComponent";
import { AddressSearchResult } from "~/lib/AddressSearch/AddressSearcher";
import FormError from "~/components/FormError/FormError";
import Autocomplete from "~/components/Autocomplete/Autocomplete";
import SpinnerIcon from "~/components/Icons/SpinnerIcon";

export interface Props {
  isSubmitting: boolean;
  addressSearchResults: AddressSearchResult[];
  onSelectAddress(id: string): void;
  managerName: FormComponent;
  managerEmail: FormComponent;
  managerPassword: FormComponent;
  restaurantName: FormComponent;
  restaurantPhoneNumber: FormComponent;
  addressLine1: FormComponent;
  addressLine2: FormComponent;
  town: FormComponent;
  postCode: FormComponent;
  step: number;
  canAdvance: boolean;
  onAdvanceStep(e: SyntheticEvent): void;
  onBackStep(e: SyntheticEvent): void;
  onSubmit(e: FormEvent): void;
  onFormKeydown(e: KeyboardEvent): void;
}

const FirstStep: FC<Props> = ({
  managerName,
  managerEmail,
  managerPassword,
  canAdvance,
  onAdvanceStep,
}) => {
  return (
    <div>
      <p className="text-gray-600 font-medium tracking-wide text-xl mt-8">
        Manager Details
      </p>

      <div className="mt-6">
        <label className="label" htmlFor="managerName">
          Manager Name <span className="text-primary">*</span>
        </label>
        <input
          {...managerName.props}
          autoFocus
          className="input"
          type="text"
          name="managerName"
          id="managerName"
        />
        <FormError component={managerName} className="mt-1" />
      </div>

      <div className="mt-4">
        <label className="label" htmlFor="managerEmail">
          Manager Email <span className="text-primary">*</span>
        </label>
        <input
          {...managerEmail.props}
          className="input"
          type="email"
          name="managerEmail"
          id="managerEmail"
          placeholder="e.g. email@email.com"
        />
        <FormError component={managerEmail} className="mt-1" />
      </div>

      <div className="mt-4">
        <label className="label" htmlFor="managerPassword">
          Manager Password <span className="text-primary">*</span>
        </label>
        <input
          {...managerPassword.props}
          className="input"
          type="password"
          name="managerPassword"
          id="managerPassword"
        />
        <FormError component={managerPassword} className="mt-1" />
      </div>

      <div className="mt-8">
        <button
          type="button"
          className="btn btn-primary font-semibold w-full"
          onClick={onAdvanceStep}
          disabled={!canAdvance}
        >
          Continue
        </button>
      </div>
    </div>
  );
};

const SecondStep: FC<Props> = ({
  restaurantName,
  restaurantPhoneNumber,
  canAdvance,
  onAdvanceStep,
  onBackStep,
}) => {
  return (
    <div>
      <p className="text-gray-600 font-medium tracking-wide text-xl mt-8">
        Restaurant Details
      </p>

      <div className="mt-6">
        <label className="label" htmlFor="restaurantName">
          Name <span className="text-primary">*</span>
        </label>
        <input
          {...restaurantName.props}
          autoFocus
          className="input"
          type="text"
          name="restaurantName"
          id="restaurantName"
        />
        <FormError component={restaurantName} className="mt-1" />
      </div>

      <div className="mt-4">
        <label className="label" htmlFor="restaurantPhoneNumber">
          Phone Number <span className="text-primary">*</span>
        </label>
        <input
          {...restaurantPhoneNumber.props}
          className="input"
          type="tel"
          name="restaurantPhoneNumber"
          id="restaurantPhoneNumber"
          placeholder="e.g. 01234 567890"
        />
        <FormError component={restaurantPhoneNumber} className="mt-1" />
      </div>

      <div className="mt-8">
        <button
          type="button"
          className="btn btn-outline-primary font-semibold w-full"
          onClick={onBackStep}
        >
          Back
        </button>
      </div>

      <div className="mt-3">
        <button
          type="button"
          className="btn btn-primary font-semibold w-full"
          onClick={onAdvanceStep}
          disabled={!canAdvance}
        >
          Continue
        </button>
      </div>
    </div>
  );
};

const LastStep: FC<Props> = ({
  isSubmitting,
  addressSearchResults,
  onSelectAddress,
  addressLine1,
  addressLine2,
  town,
  postCode,
  canAdvance,
  onBackStep,
  onSubmit,
}) => {
  return (
    <div>
      <p className="text-gray-600 font-medium tracking-wide text-xl mt-8">
        Restaurant Address
      </p>

      <div className="mt-4">
        <label className="label" htmlFor="addressLine1">
          Address Line 1 <span className="text-primary">*</span>
        </label>
        {/* @ts-ignore */}
        <Autocomplete
          predictions={addressSearchResults}
          onSelect={onSelectAddress}
        >
          {/* @ts-ignore */}
          <input
            {...addressLine1.props}
            autoFocus
            className="input"
            type="text"
            name="addressLine1"
            id="addressLine1"
            placeholder="e.g. 123 High Street"
            autoComplete="new-password"
          />
        </Autocomplete>
        <FormError component={addressLine1} className="mt-1" />
      </div>

      <div className="mt-4">
        <label className="label" htmlFor="addressLine2">
          Address Line 2
        </label>
        <input
          {...addressLine2.props}
          className="input"
          type="text"
          name="addressLine2"
          id="addressLine2"
        />
        <FormError component={addressLine2} className="mt-1" />
      </div>

      <div className="mt-4">
        <label className="label" htmlFor="town">
          Town / City <span className="text-primary">*</span>
        </label>
        <input
          {...town.props}
          className="input"
          type="text"
          name="town"
          id="town"
          placeholder="e.g. Manchester"
        />
        <FormError component={town} className="mt-1" />
      </div>

      <div className="mt-4">
        <label className="label" htmlFor="postCode">
          Post Code <span className="text-primary">*</span>
        </label>
        <input
          {...postCode.props}
          className="input"
          type="text"
          name="postCode"
          id="postCode"
          placeholder="e.g. AB12 3CD"
        />
        <FormError component={postCode} className="mt-1" />
      </div>

      <div className="mt-8">
        <button
          type="button"
          className="btn btn-outline-primary font-semibold w-full"
          onClick={onBackStep}
          disabled={!canAdvance}
        >
          Back
        </button>
      </div>

      <div className="mt-3">
        <button
          type="submit"
          className="btn btn-primary font-semibold w-full"
          onClick={onSubmit}
          disabled={!canAdvance}
        >
          {isSubmitting ? (
            <SpinnerIcon className="fill-current h-6 w-6 inline-block animate-spin" />
          ) : (
            <span>Register</span>
          )}
        </button>
      </div>
    </div>
  );
};

const RegisterRestaurantForm: FC<Props> = (props: Props) => {
  const { step, onSubmit, onFormKeydown } = props;

  return (
    <form
      action="/restaurants/register"
      method="POST"
      onSubmit={onSubmit}
      onKeyDown={onFormKeydown}
    >
      {step === 1 && <FirstStep {...props} />}
      {step === 2 && <SecondStep {...props} />}
      {step === 3 && <LastStep {...props} />}
    </form>
  );
};

export default RegisterRestaurantForm;
