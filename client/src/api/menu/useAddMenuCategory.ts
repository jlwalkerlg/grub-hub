import { useMutation, useQueryCache } from "react-query";
import Api, { ApiError } from "../api";
import { getRestaurantQueryKey } from "../restaurants/useRestaurant";

export interface AddMenuCategoryCommand {
  restaurantId: string;
  name: string;
}

async function addMenuCategory(command: AddMenuCategoryCommand) {
  const { restaurantId, ...data } = command;

  await Api.post(`/restaurants/${restaurantId}/menu/categories`, data);
}

export default function useAddMenuCategory() {
  const queryCache = useQueryCache();

  return useMutation<void, ApiError, AddMenuCategoryCommand, null>(
    addMenuCategory,
    {
      onSuccess: (_, command) => {
        queryCache.invalidateQueries(
          getRestaurantQueryKey(command.restaurantId)
        );
      },
    }
  );
}
