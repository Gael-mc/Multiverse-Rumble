// Multiverse Rumble - Motor de combate 2D
// Física simple, hitboxes AABB, barras de vida y un escenario dibujado por código.
// Si los sprites (SpriteIdleUrl / SpriteAtaqueUrl) no cargan, se dibuja un bloque de
// color de respaldo con los colores del personaje (ColorPrincipal / ColorSecundario).

(function () {
    'use strict';

    var canvas = document.getElementById('arena-canvas');
    if (!canvas || !window.BATTLE_DATA) return;

    var ctx = canvas.getContext('2d');
    ctx.imageSmoothingEnabled = false; // sprites de pixel art nítidos, sin difuminado
    var W = canvas.width;
    var H = canvas.height;

    // ---- Constantes de física / gameplay ----
    var GROUND_Y = 340;
    var GRAVITY = 0.7;
    var JUMP_FORCE = -14;
    var MOVE_SPEED = 4.2;
    var STAGE_MIN_X = 10;
    var STAGE_MAX_X = W - 10;
    var ATTACK_TOTAL_FRAMES = 16;
    var ATTACK_ACTIVE_START = 4;
    var ATTACK_ACTIVE_END = 9;
    var ATTACK_COOLDOWN_FRAMES = 14;
    var ATTACK_REACH = 34;
    var FIGHTER_WIDTH = 46;
    var FIGHTER_HEIGHT = 90;

    var data = window.BATTLE_DATA;

    var CONTROLS_P1 = { left: 'a', right: 'd', up: 'w', attack: 'f' };
    var CONTROLS_P2 = { left: 'ArrowLeft', right: 'ArrowRight', up: 'ArrowUp', attack: 'l' };

    // ---- Carga de sprites con respaldo a bloques de color ----
    function loadSprite(url) {
        if (!url) return null;
        var img = new Image();
        var failed = false;
        img.onerror = function () { failed = true; };
        img.src = url;
        return {
            img: img,
            get loaded() {
                return !failed && img.complete && img.naturalWidth > 0;
            }
        };
    }

    function rectsOverlap(a, b) {
        return a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y;
    }

    // ---- Peleador ----
    function Fighter(cfg, controls, startX, facing) {
        this.nombre = cfg.nombre;
        this.color = cfg.colorPrincipal || '#888888';
        this.colorSecundario = cfg.colorSecundario || '#333333';
        this.spriteIdle = loadSprite(cfg.spriteIdle);
        this.spriteAtaque = loadSprite(cfg.spriteAtaque);
        this.spriteSalto = loadSprite(cfg.spriteSalto);
        this.spriteCaminar = loadSprite(cfg.spriteCaminar);

        this.maxVida = cfg.vida > 0 ? cfg.vida : 100;
        this.vida = this.maxVida;
        this.ataque = cfg.ataque > 0 ? cfg.ataque : 10;
        this.defensa = cfg.defensa >= 0 ? cfg.defensa : 5;

        this.width = FIGHTER_WIDTH;
        this.height = FIGHTER_HEIGHT;

        this.x = startX;
        this.y = GROUND_Y - this.height;
        this.vx = 0;
        this.vy = 0;
        this.grounded = true;
        this.facing = facing; // 1 = derecha, -1 = izquierda

        this.attacking = false;
        this.attackTimer = 0;
        this.attackCooldown = 0;
        this.hasHitThisAttack = false;
        this.hitFlash = 0;

        this.controls = controls;
        this.alive = true;
    }

    Fighter.prototype.currentHeight = function () {
        return this.height;
    };

    Fighter.prototype.getHurtbox = function () {
        var h = this.currentHeight();
        return { x: this.x, y: this.y, w: this.width, h: h };
    };

    Fighter.prototype.getAttackBox = function () {
        if (!this.attacking) return null;
        var active = this.attackTimer >= ATTACK_ACTIVE_START && this.attackTimer <= ATTACK_ACTIVE_END;
        if (!active) return null;
        var bx = this.facing === 1 ? this.x + this.width : this.x - ATTACK_REACH;
        return { x: bx, y: this.y + 20, w: ATTACK_REACH, h: 28 };
    };

    Fighter.prototype.startAttack = function () {
        if (this.attacking || this.attackCooldown > 0 || !this.alive) return;
        this.attacking = true;
        this.attackTimer = 0;
        this.hasHitThisAttack = false;
    };

    Fighter.prototype.takeDamage = function (dmg) {
        this.vida = Math.max(0, this.vida - dmg);
        this.hitFlash = 8;
        if (this.vida <= 0) this.alive = false;
    };

    Fighter.prototype.update = function (keys, opponent) {
        if (!this.alive) return;
        var c = this.controls;

        // Temporizador de ataque / enfriamiento
        if (this.attacking) {
            this.attackTimer++;
            if (this.attackTimer > ATTACK_TOTAL_FRAMES) {
                this.attacking = false;
                this.attackCooldown = ATTACK_COOLDOWN_FRAMES;
            }
        } else if (this.attackCooldown > 0) {
            this.attackCooldown--;
        }

        // Movimiento lateral (bloqueado durante ataque)
        this.vx = 0;
        if (!this.attacking) {
            if (keys[c.left]) { this.vx = -MOVE_SPEED; this.facing = -1; }
            if (keys[c.right]) { this.vx = MOVE_SPEED; this.facing = 1; }
        }

        // Mirar automáticamente hacia el oponente cuando no se mueve ni ataca
        if (!this.attacking && this.vx === 0) {
            var miCentro = this.x + this.width / 2;
            var suCentro = opponent.x + opponent.width / 2;
            this.facing = suCentro >= miCentro ? 1 : -1;
        }

        // Salto
        if (!this.attacking && keys[c.up] && this.grounded) {
            this.vy = JUMP_FORCE;
            this.grounded = false;
        }

        // Ataque
        if (keys[c.attack]) this.startAttack();

        // Física: gravedad y posición
        this.vy += GRAVITY;
        this.x += this.vx;
        this.y += this.vy;

        // Colisión con el suelo
        var floorY = GROUND_Y - this.currentHeight();
        if (this.y >= floorY) {
            this.y = floorY;
            this.vy = 0;
            this.grounded = true;
        } else {
            this.grounded = false;
        }

        // Límites del escenario
        if (this.x < STAGE_MIN_X) this.x = STAGE_MIN_X;
        if (this.x + this.width > STAGE_MAX_X) this.x = STAGE_MAX_X - this.width;

        if (this.hitFlash > 0) this.hitFlash--;

        // Detección de golpe (una sola vez por ataque)
        var box = this.getAttackBox();
        if (box && !this.hasHitThisAttack && opponent.alive) {
            var hurt = opponent.getHurtbox();
            if (rectsOverlap(box, hurt)) {
                var dmg = Math.max(4, this.ataque - Math.floor(opponent.defensa / 2));
                opponent.takeDamage(dmg);
                this.hasHitThisAttack = true;
            }
        }
    };

    Fighter.prototype.draw = function (ctx) {
        var h = this.currentHeight();
        var y = this.y;

        // Prioridad de sprite: ataque > salto (solo si está en el aire) >
        // caminar (solo si se mueve hacia adelante/atrás en el suelo) > reposo.
        var sprite;
        if (this.attacking) {
            sprite = this.spriteAtaque;
        } else if (!this.grounded && this.spriteSalto && this.spriteSalto.loaded) {
            sprite = this.spriteSalto;
        } else if (this.grounded && this.vx !== 0 && this.spriteCaminar && this.spriteCaminar.loaded) {
            sprite = this.spriteCaminar;
        } else {
            sprite = this.spriteIdle;
        }
        if (sprite && sprite.loaded) {
            // Se respeta la proporción real de la imagen (no se estira): se ajusta
            // al alto del personaje, se centra horizontalmente y se apoya en el piso.
            var iw = sprite.img.naturalWidth || sprite.img.width;
            var ih = sprite.img.naturalHeight || sprite.img.height;
            var drawH = h;
            var drawW = iw * (drawH / ih);
            var maxW = this.width * 1.7;
            if (drawW > maxW) {
                drawW = maxW;
                drawH = ih * (drawW / iw);
            }
            var drawX = this.x + (this.width - drawW) / 2;
            var drawY = y + (h - drawH);

            ctx.save();
            if (this.facing === -1) {
                ctx.translate(drawX + drawW, drawY);
                ctx.scale(-1, 1);
                ctx.drawImage(sprite.img, 0, 0, drawW, drawH);
            } else {
                ctx.drawImage(sprite.img, drawX, drawY, drawW, drawH);
            }
            ctx.restore();
        } else {
            // Bloque de color de respaldo (mientras no haya sprites de pixel art)
            ctx.fillStyle = this.hitFlash > 0 ? '#ffffff' : this.color;
            ctx.fillRect(this.x, y, this.width, h);

            ctx.fillStyle = this.colorSecundario;
            ctx.fillRect(this.x, y + h * 0.55, this.width, h * 0.18);

            // "cabeza"
            ctx.fillStyle = this.color;
            ctx.fillRect(this.x + this.width * 0.2, y - 14, this.width * 0.6, 16);

            // indicador de dirección (ojos)
            ctx.fillStyle = '#000';
            var eyeX = this.facing === 1 ? this.x + this.width * 0.55 : this.x + this.width * 0.15;
            ctx.fillRect(eyeX, y - 8, 5, 4);

            // puño/pie extendido durante el ataque
            if (this.attacking) {
                var reach = this.facing === 1 ? this.x + this.width : this.x - 16;
                ctx.fillStyle = '#ffe08a';
                ctx.fillRect(reach, y + h * 0.3, 16, 12);
            }
        }

        // Nombre
        ctx.fillStyle = '#fff';
        ctx.font = '12px sans-serif';
        ctx.textAlign = 'center';
        ctx.fillText(this.nombre, this.x + this.width / 2, y - 18);
    };

    // ---- Fondo real del escenario (si existe), con respaldo dibujado por código ----
    var fondoEscenario = loadSprite(data.escenario && data.escenario.imagenFondo);

    function drawEscenario() {
        if (fondoEscenario && fondoEscenario.loaded) {
            ctx.drawImage(fondoEscenario.img, 0, 0, W, H);
            return;
        }
        drawEscenarioPorCodigo();
    }

    function drawEscenarioPorCodigo() {
        var skyGrad = ctx.createLinearGradient(0, 0, 0, GROUND_Y);
        skyGrad.addColorStop(0, '#8ec9f0');
        skyGrad.addColorStop(1, '#cdeafd');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, W, GROUND_Y);

        // sol
        ctx.fillStyle = '#fff7cc';
        ctx.beginPath();
        ctx.arc(W - 60, 50, 26, 0, Math.PI * 2);
        ctx.fill();

        var universo = (data.escenario && data.escenario.universo || '').toLowerCase();

        if (universo.indexOf('naruto') !== -1) {
            // Bosque de Konoha: suelo verde con siluetas de árboles
            ctx.fillStyle = '#4c9a4c';
            ctx.fillRect(0, GROUND_Y, W, H - GROUND_Y);
            ctx.fillStyle = '#3d7d3d';
            for (var gx = 0; gx < W; gx += 40) {
                ctx.fillRect(gx, GROUND_Y, 20, H - GROUND_Y);
            }
            for (var tx = 20; tx < W; tx += 150) {
                ctx.fillStyle = '#5b3a21';
                ctx.fillRect(tx - 5, GROUND_Y - 34, 10, 34);
                ctx.fillStyle = '#2f5d2f';
                ctx.beginPath();
                ctx.arc(tx, GROUND_Y - 60, 26, 0, Math.PI * 2);
                ctx.fill();
            }
        } else {
            // Torneo de las Artes Marciales: piso de baldosas grises
            var tile = 40;
            var row = 0;
            for (var ty = GROUND_Y; ty < H; ty += tile, row++) {
                var col = 0;
                for (var txx = 0; txx < W; txx += tile, col++) {
                    ctx.fillStyle = (row + col) % 2 === 0 ? '#c9c9c9' : '#a8a8a8';
                    ctx.fillRect(txx, ty, tile, tile);
                }
            }
            ctx.strokeStyle = '#e0393b';
            ctx.lineWidth = 4;
            ctx.strokeRect(4, GROUND_Y + 2, W - 8, H - GROUND_Y - 6);
        }
    }

    function drawBarClasica(x, y, fighter, flip) {
        var w = 200, h = 18;
        ctx.fillStyle = '#222';
        ctx.fillRect(x - 2, y - 2, w + 4, h + 4);
        ctx.fillStyle = '#601515';
        ctx.fillRect(x, y, w, h);

        var pct = Math.max(0, fighter.vida / fighter.maxVida);
        ctx.fillStyle = pct > 0.5 ? '#3fcf5a' : (pct > 0.2 ? '#e6b800' : '#d92727');
        if (flip) {
            ctx.fillRect(x + w * (1 - pct), y, w * pct, h);
        } else {
            ctx.fillRect(x, y, w * pct, h);
        }

        ctx.fillStyle = '#fff';
        ctx.font = 'bold 13px sans-serif';
        ctx.textAlign = flip ? 'right' : 'left';
        ctx.fillText(fighter.nombre, flip ? x + w : x, y - 6);
    }

    // ---- Medidor de vida "Power Meter" (sprites SM64DS "POWER") ----
    // 9 cuadros que van de lleno (azul) a vacío (rojo oscuro), igual que el
    // medidor original. Si no cargan, se usa la barra clásica de respaldo.
    var POWER_METER_FRAMES = [];
    for (var pmIdx = 1; pmIdx <= 9; pmIdx++) {
        POWER_METER_FRAMES.push(loadSprite('/img/ui/power_meter_' + pmIdx + '.png'));
    }

    function powerMeterListo() {
        for (var i = 0; i < POWER_METER_FRAMES.length; i++) {
            if (!POWER_METER_FRAMES[i] || !POWER_METER_FRAMES[i].loaded) return false;
        }
        return true;
    }

    function drawPowerMeter(x, y, fighter, flip) {
        var pct = Math.max(0, fighter.vida / fighter.maxVida);
        var idx = Math.round((1 - pct) * (POWER_METER_FRAMES.length - 1));
        idx = Math.max(0, Math.min(POWER_METER_FRAMES.length - 1, idx));
        var frame = POWER_METER_FRAMES[idx];

        var size = 66;
        var drawX = flip ? x - size : x;

        ctx.drawImage(frame.img, drawX, y, size, size);

        ctx.fillStyle = '#fff';
        ctx.font = 'bold 13px sans-serif';
        ctx.textAlign = flip ? 'right' : 'left';
        ctx.fillText(fighter.nombre, flip ? drawX + size : drawX, y + size + 14);
    }

    function drawHealthBars() {
        if (powerMeterListo()) {
            drawPowerMeter(16, 8, player1, false);
            drawPowerMeter(W - 16, 8, player2, true);
        } else {
            drawBarClasica(20, 16, player1, false);
            drawBarClasica(W - 220, 16, player2, true);
        }
    }

    // ---- Entradas de teclado ----
    var keys = {};

    function normalizeKey(k) {
        return k.length === 1 ? k.toLowerCase() : k;
    }

    var ARROW_KEYS = ['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown'];

    window.addEventListener('keydown', function (e) {
        keys[normalizeKey(e.key)] = true;
        if (ARROW_KEYS.indexOf(e.key) !== -1) e.preventDefault();
    });
    window.addEventListener('keyup', function (e) {
        keys[normalizeKey(e.key)] = false;
    });

    // ---- Peleadores ----
    var player1 = new Fighter(data.p1, CONTROLS_P1, 150, 1);
    var player2 = new Fighter(data.p2, CONTROLS_P2, 600, -1);

    var gameOver = false;
    var winner = null;

    function checkGameOver() {
        if (gameOver) return;
        if (!player1.alive || !player2.alive) {
            gameOver = true;
            if (!player1.alive && !player2.alive) {
                winner = 'Empate';
            } else {
                winner = player1.alive ? player1.nombre : player2.nombre;
            }
            showResultOverlay();
        }
    }

    function showResultOverlay() {
        var overlay = document.getElementById('resultado-overlay');
        var texto = document.getElementById('resultado-texto');
        var input = document.getElementById('input-ganador');
        if (texto) {
            texto.textContent = winner === 'Empate' ? '¡Empate!' : ('¡' + winner + ' gana el combate!');
        }
        if (input) input.value = winner;
        if (overlay) overlay.classList.remove('d-none');
    }

    // ---- Bucle principal ----
    function loop() {
        if (!gameOver) {
            player1.update(keys, player2);
            player2.update(keys, player1);
            checkGameOver();
        }

        ctx.clearRect(0, 0, W, H);
        drawEscenario();
        player1.draw(ctx);
        player2.draw(ctx);
        drawHealthBars();

        requestAnimationFrame(loop);
    }

    requestAnimationFrame(loop);
})();
