export interface Node {
    x: number;
    y: number;
    vx: number;
    vy: number;
    radius: number;
    opacity: number;
}

export interface Particle {
    from: Node;
    to: Node;
    progress: number;
    speed: number;
    radius: number;
    opacity: number;
}

const getRandom = (min: number, max: number): number => {
    return Math.random() * (max - min) + min;
};

export const startCanvasAnimation = (canvas: HTMLCanvasElement) => {
    const ctx = canvas.getContext('2d')!;
    let animationFrameId: number;
    let frameCounter = 0;
    let nodeSpreadMultiplier = 1;

    let width: number, height: number;
    let nodes: Node[] = [], connections: { from: Node; to: Node }[] = [], particles: Particle[] = [];

    const resizeCanvas = () => {
        const dpr = window.devicePixelRatio || 1;
        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;
        ctx.setTransform(1, 0, 0, 1, 0, 0);
        ctx.scale(dpr, dpr);
        width = rect.width;
        height = rect.height;
    };

    const initScene = () => {
        resizeCanvas();
        const isMobile: boolean = width < 768;
        const area = width * height;
        const DENSITY_FACTOR = isMobile ? 12000 : 15000;
        const NODE_COUNT = Math.max(40, Math.min(160, Math.floor(area / DENSITY_FACTOR)));
        const PARTICLE_COUNT = Math.floor(NODE_COUNT / 4);
        const BASE_RADIUS = isMobile ? width * 0.35 : width * 0.45;
        const MAX_DIST = isMobile ? 160 : 170;

        nodes = Array.from({length: NODE_COUNT}, () => {
            const angle = getRandom(0, 2 * Math.PI);
            const radius = getRandom(0, BASE_RADIUS) * Math.sqrt(Math.random());
            return {
                x: width / 2 + Math.cos(angle) * radius,
                y: height / 2 + Math.sin(angle) * radius,
                vx: getRandom(-0.05, 0.05),
                vy: getRandom(-0.05, 0.05),
                radius: getRandom(1, 2.5),
                opacity: getRandom(0.2, 0.7),
            };
        });

        connections = [];
        for (let i = 0; i < nodes.length; i++) {
            for (let j = i + 1; j < nodes.length; j++) {
                const dist = Math.hypot(nodes[i].x - nodes[j].x, nodes[i].y - nodes[j].y);
                if (dist < MAX_DIST && Math.random() > 0.9) {
                    connections.push({from: nodes[i], to: nodes[j]});
                }
            }
        }

        particles = [];
        for (let i = 0; i < PARTICLE_COUNT; i++) {
            const conn = connections[Math.floor(Math.random() * connections.length)];
            particles.push({
                from: conn.from,
                to: conn.to,
                progress: Math.random(),
                speed: getRandom(0.002, 0.006),
                radius: getRandom(1, 2),
                opacity: getRandom(0.6, 1),
            });
        }
    };

    const animate = () => {
        ctx.clearRect(0, 0, width, height);
        frameCounter++;

        if (frameCounter % (60 * 20) === 0) {
            nodeSpreadMultiplier = Math.min(2, nodeSpreadMultiplier + 0.015);
        }

        for (const node of nodes) {
            node.x += node.vx;
            node.y += node.vy;
            if (node.x < 0 || node.x > width) node.vx *= -1;
            if (node.y < 0 || node.y > height) node.vy *= -1;

            const maxSpread = 0.006;
            const growthRate = 0.0002;
            nodeSpreadMultiplier = Math.min(maxSpread, nodeSpreadMultiplier + growthRate);

            const dx = node.x - width / 2;
            const dy = node.y - height / 2;
            const distance = Math.sqrt(dx * dx + dy * dy);
            const directionX = dx / distance || 0;
            const directionY = dy / distance || 0;

            node.x += directionX * nodeSpreadMultiplier;
            node.y += directionY * nodeSpreadMultiplier;


            ctx.beginPath();
            ctx.arc(node.x, node.y, node.radius, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(180, 220, 255, ${node.opacity})`;
            ctx.fill();
        }

        for (const conn of connections) {
            ctx.beginPath();
            ctx.moveTo(conn.from.x, conn.from.y);
            ctx.lineTo(conn.to.x, conn.to.y);
            ctx.strokeStyle = 'rgba(135,206,250,0.1)';
            ctx.lineWidth = 0.7;
            ctx.stroke();
        }

        for (const p of particles) {
            p.progress += p.speed;
            if (p.progress > 1) {
                p.progress = 0;
                const newConn = connections[Math.floor(Math.random() * connections.length)];
                p.from = newConn.from;
                p.to = newConn.to;
            }

            const x = p.from.x + (p.to.x - p.from.x) * p.progress;
            const y = p.from.y + (p.to.y - p.from.y) * p.progress;

            const gradient = ctx.createRadialGradient(x, y, 0, x, y, p.radius * 6);
            gradient.addColorStop(0, `rgba(0, 255, 255, ${p.opacity})`);
            gradient.addColorStop(0.3, `rgba(0, 255, 255, ${p.opacity * 0.5})`);
            gradient.addColorStop(1, `rgba(0, 255, 255, 0)`);

            ctx.fillStyle = gradient;
            ctx.beginPath();
            ctx.arc(x, y, p.radius * 6, 0, Math.PI * 2);
            ctx.fill();

            ctx.beginPath();
            ctx.arc(x, y, p.radius, 0, Math.PI * 2);
            ctx.fillStyle = `rgba(0,255,255,${p.opacity})`;
            ctx.fill();
        }

        animationFrameId = requestAnimationFrame(animate);
    };

    const handleResize = () => {
        cancelAnimationFrame(animationFrameId);
        nodeSpreadMultiplier = 1;
        frameCounter = 0;
        initScene();
        animate();
    };

    window.addEventListener('resize', handleResize);

    initScene();
    animate();

    return () => {
        cancelAnimationFrame(animationFrameId);
        window.removeEventListener('resize', handleResize);
    };
};
