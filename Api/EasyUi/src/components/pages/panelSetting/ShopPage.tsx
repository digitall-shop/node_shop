import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { useTranslation } from 'react-i18next';

const mockItems = [
    {
        id: 1,
        name: 'Lenovo Laptop',
        description: 'Suitable for everyday work and light graphics',
        variants: [
            { id: '1a', title: 'Core i5 / 8GB / 256SSD', price: '25,000,000 Tomans' },
            { id: '1b', title: 'Core i7 / 16GB / 512SSD', price: '35,000,000 Tomans' }
        ]
    },
    {
        id: 2,
        name: 'Wireless Headphones',
        description: 'Excellent sound quality with noise cancellation',
        variants: [
            { id: '2a', title: 'Black', price: '1,450,000 Tomans' },
            { id: '2b', title: 'White', price: '1,500,000 Tomans' }
        ]

    },
    {
        id: 3,
        name: 'Wireless Headphones',
        description: 'Excellent sound quality with noise cancellation',
        variants: [
            { id: '3a', title: 'Black', price: '1,450,000 Tomans' },
            { id: '3b', title: 'White', price: '1,500,000 Tomans' }
        ]

    }
];

const ShopPage: React.FC = () => {
    const { t } = useTranslation();
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const timer = setTimeout(() => setIsLoading(false), 2000);
        return () => clearTimeout(timer);
    }, []);

    return (
        <div className="p-4 sm:p-6 md:p-8 max-w-6xl mx-auto pb-10">
            <div className="space-y-6">
                {isLoading
                    ? Array.from({ length: 2 }).map((_, i) => (
                        <div
                            key={i}
                            className="animate-pulse bg-gray-800/40 border border-gray-700/60 rounded-xl p-6 space-y-4"
                        >
                            <div className="h-6 bg-gray-700 rounded w-1/3" />
                            <div className="h-4 bg-gray-700 rounded w-1/2" />
                            <div className="space-y-2 pt-2">
                                <div className="h-4 bg-gray-700 rounded w-full" />
                                <div className="h-4 bg-gray-700 rounded w-3/4" />
                            </div>
                        </div>
                    ))
                    : mockItems.map((product) => (
                        <motion.div
                            key={product.id}
                            className="bg-gray-800/50 backdrop-blur-sm border border-gray-700/60 rounded-xl p-6 space-y-4 shadow-lg"
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ duration: 0.4 }}
                        >
                            <div>
                                <h2 className="text-white font-bold text-lg">{product.name}</h2>
                                <p className="text-sm text-gray-400">{product.description}</p>
                            </div>
                            <div className="space-y-3">
                                {product.variants.map((variant) => (
                                    <div
                                        key={variant.id}
                                        className="flex justify-between items-center bg-gray-700/30 p-3 rounded-md"
                                    >
                                        <div>
                                            <p className="text-white text-sm">{variant.title}</p>
                                            <p className="text-green-400 text-xs mt-1">{variant.price}</p>
                                        </div>
                                        <motion.button
                                            whileHover={{ scale: 1.05 }}
                                            whileTap={{ scale: 0.95 }}
                                            className="px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-sm text-white rounded-lg transition-all"
                                            onClick={() => alert(`Buy: ${product.name} - ${variant.title}`)}
                                        >
                                            {t('shop.buy', 'Buy')}
                                        </motion.button>
                                    </div>
                                ))}
                            </div>
                        </motion.div>
                    ))}
            </div>
        </div>
    );
};

export default ShopPage;
