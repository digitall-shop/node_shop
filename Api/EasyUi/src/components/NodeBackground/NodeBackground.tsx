import React, {useRef, useEffect} from 'react';
import './nodeBackgroundStyles.css';
import {startCanvasAnimation} from './nodeBackgroundLogic';

const NodeBackground: React.FC = () => {
    const canvasRef = useRef<HTMLCanvasElement | null>(null);

    useEffect(() => {
        if (!canvasRef.current) return;
        const cleanup = startCanvasAnimation(canvasRef.current);
        return cleanup;
    }, []);

    return <canvas ref={canvasRef} className="node-background-canvas"/>;
};

export default NodeBackground;
