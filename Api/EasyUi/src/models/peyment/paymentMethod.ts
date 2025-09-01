export const PaymentMethod = { CardToCard: 0, Plisio: 1 } as const;
export type PaymentMethod = typeof PaymentMethod[keyof typeof PaymentMethod];
