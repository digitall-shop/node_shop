import React, { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const mockGroups = [
    {
        id: 'basic',
        label: 'BASIC',
        hosts: ['192.168.0.1', '192.168.0.2']
    },
    {
        id: 'limited',
        label: 'LIMITED_PROXY',
        hosts: []
    },
    {
        id: 'expired',
        label: 'EXPIRED_PROXY',
        hosts: ['10.0.0.5']
    },
    {
        id: 'disabled',
        label: 'DISABLED_PROXY',
        hosts: []
    },
    {
        id: 'nodeshop',
        label: 'NodeShop-DE_Test',
        hosts: ['test', '91.107.249.231']
    }
];

const HostPage: React.FC = () => {
    const [expandedGroup, setExpandedGroup] = useState<string | null>(null);

    const toggleGroup = (id: string) => {
        setExpandedGroup(prev => (prev === id ? null : id));
    };

    return (
        <div className="max-w-2xl mx-auto p-6 space-y-4">
            {mockGroups.map(group => (
                <div key={group.id} className="border border-gray-700 rounded-xl bg-gray-800/50 backdrop-blur-sm shadow-md">
                    <button
                        onClick={() => toggleGroup(group.id)}
                        className="w-full flex justify-between items-center px-4 py-3 hover:bg-gray-700/40 transition-colors text-white font-semibold"
                    >
                        <span>{group.label}</span>
                        <motion.i
                            animate={{ rotate: expandedGroup === group.id ? 180 : 0 }}
                            className="fas fa-chevron-down text-sm text-gray-400"
                        ></motion.i>
                    </button>

                    <AnimatePresence>
                        {expandedGroup === group.id && (
                            <motion.div
                                initial={{ height: 0, opacity: 0 }}
                                animate={{ height: 'auto', opacity: 1 }}
                                exit={{ height: 0, opacity: 0 }}
                                className="px-4 pb-4 pt-2 space-y-3"
                            >
                                {group.hosts.map((host, index) => (
                                    <div key={index} className="flex items-center justify-between bg-gray-700/30 px-3 py-2 rounded-md">
                                        <span className="text-white text-sm">{host}</span>
                                        <div className="flex items-center gap-2 text-gray-300">
                                            <button className="hover:text-blue-400" title="Edit">
                                                <i className="fas fa-edit text-sm"></i>
                                            </button>
                                            <button className="hover:text-red-400" title="Delete">
                                                <i className="fas fa-trash-alt text-sm"></i>
                                            </button>
                                        </div>
                                    </div>
                                ))}
                                <input
                                    type="text"
                                    placeholder="Add host"
                                    className="w-full px-3 py-2 rounded-md bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />
                            </motion.div>
                        )}
                    </AnimatePresence>
                </div>
            ))}

            <div className="pt-4">
                <button className="w-full py-2 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-lg transition">
                    Apply
                </button>
            </div>
        </div>
    );
};

export default HostPage;
