export interface BankAccount {
    id: number;
    isDeleted: boolean;
    bankName: string | null;
    holderName: string | null;
    cardNumber: string | null;
    isActive: boolean;
}

export interface CreateBankAccountDto {
    bankName?: string | null;
    holderName?: string | null;
    cardNumber?: string | null;
    isActive: boolean;
}

export interface UpdateBankAccountDto {
    bankName?: string | null;
    holderName?: string | null;
    cardNumber?: string | null;
    isActive?: boolean;
}
