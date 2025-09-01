import React, {useState} from "react";
import {AnimatePresence, motion} from "framer-motion";
import {FaChevronDown, FaFilter, FaUndo} from "react-icons/fa";
import {useTranslation} from "react-i18next";
import {useUpdateBalance} from "../../hooks/useUpdateBalance";
import {useToggleBlock} from "../../hooks/useToggleBlock";
import {useSendMessage} from "../../hooks/useSendMessage";
import {ITEMS_PER_PAGE} from "../../hooks/useAdminPayments";
import {useUsersQuery} from "../../hooks/useUsersQuery";
import type {GetUsersParams, User} from "../../models/user/user";
import {FilterInput} from "./user/FilterInput";
import {UserAccordionCard} from "./user/UserAccordionCard";
import {SkeletonUserCard} from "./user/SkeletonUserCard";
import {EmptyState} from "./user/EmptyState";
import {Pagination} from "./user/Pagination";
import {BalanceModal} from "./user/BalanceModal";
import {SendMessageModal} from "./user/SendMessageModal.tsx";


type FormInputs = { amount: string; description: string };

const Users: React.FC = () => {
    const {t} = useTranslation();

    const [page, setPage] = useState(1);
    const [filters, setFilters] = useState<Partial<GetUsersParams>>({});
    const [isFiltersOpen, setIsFiltersOpen] = useState(false);
    const [isPageSwitch, setIsPageSwitch] = useState(false);
    const [expandedUserId, setExpandedUserId] = useState<number | null>(null);

    const [balanceModal, setBalanceModal] = useState<{
        isOpen: boolean;
        user: User | null;
        type: "credit" | "debit" | null
    }>({
        isOpen: false,
        user: null,
        type: null,
    });
    const [messageModal, setMessageModal] = useState<{ isOpen: boolean; user: User | null }>({
        isOpen: false,
        user: null
    });

    const {data, isLoading, isError, isFetching} = useUsersQuery(filters, page);
    const totalCount = data?.totalCount ?? 0;
    const totalPages = Math.ceil(totalCount / ITEMS_PER_PAGE);

    const updateBalance = useUpdateBalance(() => setBalanceModal({isOpen: false, user: null, type: null}));
    const sendMessage = useSendMessage(() => setMessageModal({isOpen: false, user: null}));
    const {toggleBlock, isToggling, busyUserId} = useToggleBlock();

    React.useEffect(() => {
        if (!isFetching) setIsPageSwitch(false);
    }, [isFetching]);
    React.useEffect(() => setIsPageSwitch(false), [data?.items?.length, page]);
    React.useEffect(() => {
        if (isPageSwitch) {
            const t = setTimeout(() => setIsPageSwitch(false), 1200);
            return () => clearTimeout(t);
        }
    }, [isPageSwitch]);

    const handleFilterChange = (field: keyof GetUsersParams, value: string) => {
        setFilters(prev => ({...prev, [field]: value || undefined}));
        setPage(1);
    };
    const handleResetFilters = () => {
        setFilters({});
        setPage(1);
    };

    const handleOpenBalance = (user: User, type: "credit" | "debit") =>
        setBalanceModal({isOpen: true, user, type});
    const handleCloseBalance = () =>
        setBalanceModal({isOpen: false, user: null, type: null});
    const handleSubmitBalanceUpdate = (formData: FormInputs) => {
        if (!balanceModal.user || !balanceModal.type) return;
        updateBalance.mutate({
            type: balanceModal.type,
            userId: balanceModal.user.id,
            amount: Number(formData.amount),
            description: formData.description ?? "",
        });
    };

    const openMessageModal = (user: User) => setMessageModal({isOpen: true, user});
    const closeMessageModal = () => setMessageModal({isOpen: false, user: null});

    const handleToggleBlock = (user: User) => {
        const username = user.userName || `ID: ${user.id}`;
        toggleBlock({ userId: user.id, block: !user.isBlocked, username });
    };


    const handlePageChange = (p: number) => {
        setIsPageSwitch(true);
        setPage(p);
    };

    return (
        <div className="container mx-auto px-3 sm:px-4 py-6 sm:py-8 text-white">
            <AnimatePresence>
                {isPageSwitch && (
                    <motion.div
                        key="top-progress"
                        initial={{scaleX: 0}}
                        animate={{scaleX: 1}}
                        exit={{scaleX: 0}}
                        transition={{duration: 0.7, ease: "easeInOut"}}
                        style={{transformOrigin: "0% 50%"}}
                        className="fixed top-0 left-0 right-0 h-1 bg-cyan-400 z-50"
                    />
                )}
            </AnimatePresence>

            <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-5 sm:mb-6">
                <h1 className="text-2xl sm:text-3xl font-bold truncate">{t("usersPage.title")}</h1>

            </div>

            <div className="mb-5 sm:mb-6">
                <motion.button
                    onClick={() => setIsFiltersOpen(!isFiltersOpen)}
                    className="w-full flex justify-between items-center px-3 sm:px-4 py-3 bg-gray-800/80 hover:bg-gray-800 rounded-lg font-semibold transition-colors"
                >
          <span className="flex items-center gap-2 whitespace-nowrap overflow-hidden text-ellipsis">
            <FaFilter/>{t("usersPage.filters.title")}
          </span>
                    <motion.div animate={{rotate: isFiltersOpen ? 180 : 0}}><FaChevronDown/></motion.div>
                </motion.button>

                <AnimatePresence>
                    {isFiltersOpen && (
                        <motion.div
                            initial={{height: 0, opacity: 0, marginTop: 0}}
                            animate={{height: "auto", opacity: 1, marginTop: "0.75rem"}}
                            exit={{height: 0, opacity: 0, marginTop: 0}}
                            className="bg-gray-800/50 backdrop-blur-sm border border-gray-700/60 rounded-xl p-3 sm:p-4 overflow-hidden"
                        >
                            <div className="grid gap-3 sm:gap-4 grid-cols-2 md:grid-cols-3 lg:grid-cols-5 items-end">
                                <FilterInput label={t("usersPage.filters.username")} placeholder="@username"
                                             onChange={(v) => handleFilterChange("userName", v)}/>
                                <FilterInput type="number" label={t("usersPage.filters.fromBalance")} placeholder="1000"
                                             onChange={(v) => handleFilterChange("fromBalance", v)}/>
                                <FilterInput type="number" label={t("usersPage.filters.toBalance")} placeholder="5000"
                                             onChange={(v) => handleFilterChange("toBalance", v)}/>
                                <div className="col-span-2 md:col-span-3 lg:col-span-2 flex items-center gap-2">
                                    <button onClick={handleResetFilters}
                                            className="w-full flex items-center justify-center gap-2 px-3 sm:px-4 py-2.5 rounded-lg bg-gray-600 hover:bg-gray-700 text-white font-semibold transition-colors">
                                        <FaUndo/>{t("usersPage.filters.reset")}
                                    </button>
                                </div>
                            </div>
                        </motion.div>
                    )}
                </AnimatePresence>
            </div>

            {isFetching && <div
                className="self-start sm:self-auto w-5 h-5 border-2 border-cyan-400 border-t-transparent justify-self-center rounded-full animate-spin"/>}
            {isError && <p className="text-center text-red-500">{t("common.error")}</p>}

            <AnimatePresence>
                {(data?.items && data.items.length > 0) || isPageSwitch ? (
                    <div className="space-y-3">
                        {isPageSwitch
                            ? Array.from({length: Math.min(ITEMS_PER_PAGE, 6)}).map((_, i) => <SkeletonUserCard
                                key={`sk-${i}`}/>)
                            : data!.items.map((user) => (
                                <UserAccordionCard
                                    key={user.id}
                                    user={user}
                                    isExpanded={expandedUserId === user.id}
                                    onToggle={() => setExpandedUserId((prev) => (prev === user.id ? null : user.id))}
                                    onCredit={() => handleOpenBalance(user, "credit")}
                                    onDebit={() => handleOpenBalance(user, "debit")}
                                    onMessage={() => openMessageModal(user)}
                                    onToggleBlock={() => handleToggleBlock(user)}
                                    toggleBusy={busyUserId === user.id && isToggling}
                                />
                            ))}
                    </div>
                ) : (
                    !isLoading && !isError && <EmptyState/>
                )}
            </AnimatePresence>

            {totalCount > 0 && (
                <Pagination
                    page={page}
                    totalPages={totalPages}
                    onPageChange={handlePageChange}
                    totalItems={totalCount}
                    isBusy={isPageSwitch || isFetching}
                />
            )}

            <AnimatePresence>
                {balanceModal.isOpen && (
                    <BalanceModal
                        user={balanceModal.user!}
                        type={balanceModal.type!}
                        onClose={handleCloseBalance}
                        onSubmit={handleSubmitBalanceUpdate}
                        isLoading={updateBalance.isPending}
                    />
                )}
            </AnimatePresence>

            <AnimatePresence>
                {messageModal.isOpen && messageModal.user && (
                    <SendMessageModal
                        user={messageModal.user}
                        isLoading={sendMessage.isPending}
                        onSend={(text) => sendMessage.mutate({userId: messageModal.user!.id, text})}
                        onClose={closeMessageModal}
                    />
                )}
            </AnimatePresence>
        </div>
    );
};

export default Users;
