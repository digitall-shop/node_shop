import React from 'react';

interface AvatarProps {
    src: string | null | undefined;
    name: string;
    className?: string;
}

const Avatar: React.FC<AvatarProps> = ({src, name, className = ''}) => {
    const initial = name ? name.charAt(0).toUpperCase() : 'U';

    if (src) {
        return (
            <img
                src={src}
                alt={`${name}'s Avatar`}
                className={`rounded-full object-cover ${className}`}
            />
        );
    }

    return (
        <div
            className={`flex items-center justify-center rounded-full bg-cyan-600 text-white font-bold ${className}`}
        >
            {initial}
        </div>
    );
};

export default Avatar;