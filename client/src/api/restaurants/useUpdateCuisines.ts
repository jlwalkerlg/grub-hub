import { useMutation, useQueryClient } from "react-query";
import Api, { ApiError } from "../api";
import { getRestaurantQueryKey } from "./useRestaurant";

export interface UpdateCuisinesRequest {
  cuisines: string[];
}

async function updateCuisines(
  restaurantId: string,
  request: UpdateCuisinesRequest
) {
  return Api.put(`/restaurants/${restaurantId}/cuisines`, request);
}

export default function useUpdateCuisines(restaurantId: string) {
  const queryClient = useQueryClient();

  return useMutation<void, ApiError, UpdateCuisinesRequest, null>(
    async (command) => {
      await updateCuisines(restaurantId, command);
    },
    {
      onSuccess: () => {
        queryClient.invalidateQueries(getRestaurantQueryKey(restaurantId));
      },
    }
  );
}
