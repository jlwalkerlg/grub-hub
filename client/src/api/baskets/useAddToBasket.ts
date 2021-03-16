import { useMutation, useQueryClient } from "react-query";
import Api, { ApiError } from "../api";
import { getBasketQueryKey } from "./useBasket";

interface AddToBasketCommand {
  restaurantId: string;
  menuItemId: string;
  quantity: number;
}

export function useAddToBasket() {
  const queryClient = useQueryClient();

  return useMutation<void, ApiError, AddToBasketCommand, null>(
    async (command) => {
      await Api.post(`/restaurants/${command.restaurantId}/basket`, command);
    },
    {
      onSuccess: (_, command) => {
        queryClient.invalidateQueries(getBasketQueryKey(command.restaurantId));
      },
    }
  );
}
