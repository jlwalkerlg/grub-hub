import { useRouter } from "next/router";
import React, { FC, useState } from "react";
import { useQueryClient } from "react-query";
import useRegisterRestaurant, {
  RegisterRestaurantCommand,
} from "~/api/restaurants/useRegisterRestaurant";
import { getAuthUser, getAuthUserQueryKey } from "~/api/users/useAuth";
import { ErrorAlert } from "~/components/Alert/Alert";
import RegisterRestaurantFormStepOne from "./RegisterRestaurantFormStepOne";
import RegisterRestaurantFormStepThree from "./RegisterRestaurantFormStepThree";
import RegisterRestaurantFormStepTwo from "./RegisterRestaurantFormStepTwo";

const RegisterRestaurantForm: FC = () => {
  const router = useRouter();

  const queryClient = useQueryClient();

  const {
    mutate: register,
    isError,
    error,
    isLoading,
    isSuccess,
  } = useRegisterRestaurant();

  const [values, setValues] = useState<RegisterRestaurantCommand>({
    managerFirstName: "Jordan",
    managerLastName: "Walker",
    managerEmail: "jordan@microworld.co.uk",
    managerPassword: "password123",
    restaurantName: "Chow Main",
    restaurantPhoneNumber: "01234567890",
    addressLine1: "19 Bodmin Avenue",
    addressLine2: "Wrose",
    city: "Shipley",
    postcode: "BD181LT",
  });

  const [errors, setErrors] = useState<
    { [K in keyof RegisterRestaurantCommand]?: string[] }
  >({});

  const [step, setStep] = useState(1);

  const advanceStep = (data: any) => {
    setValues({ ...values, ...data });
    setStep(step + 1);
  };

  const backStep = (data: any) => {
    setValues({ ...values, ...data });
    setStep(step - 1);
  };

  const onSubmit = async (data: any) => {
    if (isLoading || isSuccess) return;

    const command = { ...values, ...data };

    setValues(command);

    register(command, {
      onSuccess: async () => {
        const user = await getAuthUser();
        queryClient.setQueryData(getAuthUserQueryKey(), user);
        localStorage.setItem("isLoggedIn", "true");

        await router.push("/dashboard");
      },

      onError: (error) => {
        if (error.isValidationError) {
          setErrors(error.errors);

          for (const field of [
            "managerFirstName",
            "managerLastName",
            "managerEmail",
            "managerPassword",
          ]) {
            if (error.errors.hasOwnProperty(field)) {
              setStep(1);
              return;
            }
          }

          for (const field of ["restaurantName", "restaurantPhoneNumber"]) {
            if (error.errors.hasOwnProperty(field)) {
              setStep(2);
              return;
            }
          }
        }
      },
    });
  };

  return (
    <div>
      {isError && (
        <div className="my-6">
          <ErrorAlert message={error?.message} />
        </div>
      )}

      {step === 1 && (
        <RegisterRestaurantFormStepOne
          defaults={values}
          errors={errors}
          advanceStep={advanceStep}
        />
      )}
      {step === 2 && (
        <RegisterRestaurantFormStepTwo
          defaults={values}
          errors={errors}
          backStep={backStep}
          advanceStep={advanceStep}
        />
      )}
      {step === 3 && (
        <RegisterRestaurantFormStepThree
          isSubmitting={isLoading}
          defaults={values}
          errors={errors}
          backStep={backStep}
          onSubmit={onSubmit}
        />
      )}
    </div>
  );
};

export default RegisterRestaurantForm;
