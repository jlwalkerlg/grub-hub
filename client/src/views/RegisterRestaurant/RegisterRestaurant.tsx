import React, { FC } from "react";
import Head from "next/head";

import Layout from "~/components/Layout/Layout";

import RegisterRestaurantFormController from "./RegisterRestaurantForm/RegisterRestaurantFormController";

const browser = typeof window !== "undefined";

export const RegisterRestaurant: FC = () => {
  return (
    <Layout title="Register Restaurant">
      <Head>
        {!browser && (
          <script
            type="text/javascript"
            src="https://maps.googleapis.com/maps/api/js?key=AIzaSyD7xEW_Qy_ILGDOuMxwBftuzze0QDGdLSQ&libraries=places"
            async
          ></script>
        )}
      </Head>
      <main>
        <div className="container mt-8">
          <div className="bg-white p-8 shadow-sm border-t-2 border-solid border-gray-300">
            <h2 className="text-2xl font-semibold text-gray-800 tracking-wider">
              Register Your Restaurant
            </h2>
            <p className="mt-2 text-gray-700">
              Enter the details of your restaurant and the restaurant&apos;s
              manager below. We will review your application and get back too
              you shortly.
            </p>

            <RegisterRestaurantFormController />
          </div>
        </div>
      </main>
    </Layout>
  );
};
