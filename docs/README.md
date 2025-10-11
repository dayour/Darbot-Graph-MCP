# Darbot Graph MCP - GitHub Pages Site

This directory contains the GitHub Pages website for Darbot Graph MCP.

## Overview

The website features:
- **Retro Cyber Modern Fluent WinUI3 Design** - Modern, gradient-based cyberpunk aesthetic with Fluent Design influences
- **Comprehensive Documentation** - Setup guides, API reference, examples, and extensibility guides
- **Interactive Clippy Assistant** - Powered by Spark LLM API to help users with setup and usage
- **Responsive Design** - Optimized for desktop, tablet, and mobile devices
- **Smooth Animations** - Fade-ins, parallax effects, and smooth transitions

## Structure

```
docs/
├── index.html              # Homepage
├── docs.html              # Documentation page
├── api.html               # API reference
├── examples.html          # Usage examples
├── extensibility.html     # Extensibility guide
├── wiki.html              # Wiki and resources
├── assets/
│   ├── css/
│   │   ├── style.css      # Main stylesheet
│   │   └── docs.css       # Documentation styles
│   ├── js/
│   │   ├── main.js        # Main JavaScript
│   │   ├── clippy.js      # Clippy assistant
│   │   └── docs.js        # Documentation page JS
│   └── images/            # Images (future)
└── _config.yml            # Jekyll configuration
```

## Design Philosophy

### Retro Cyber Modern Aesthetic
- **Color Palette**: Purple/pink gradients (#667eea → #764ba2)
- **Cyber Accents**: Neon blue (#00d4ff), cyber pink (#ff006e), cyber green (#06ffa5)
- **Fluent Design**: Acrylic surfaces, depth, motion, and material

### Key Features
1. **Hero Section**: Eye-catching gradient backgrounds with animated pulse effects
2. **Feature Cards**: Hover effects with smooth transitions
3. **Floating Clippy**: AI-powered assistant for user help
4. **Navigation**: Sticky navigation bar with smooth scrolling
5. **Code Blocks**: Dark theme code blocks with copy functionality

## Clippy Assistant

The Clippy assistant is powered by a knowledge base and Spark LLM API integration:
- Knowledge base responses for common questions
- Context-aware help for setup, tools, and extensibility
- Floating widget that can be toggled
- Simulated typing indicator for better UX

## Local Development

To test locally:
1. Open `index.html` in a web browser
2. Use a local server for best results:
   ```bash
   python -m http.server 8000
   # Or
   npx serve
   ```
3. Navigate to `http://localhost:8000`

## Deployment

GitHub Pages automatically serves this site from the `docs/` directory.

To enable:
1. Go to repository Settings → Pages
2. Set source to "Deploy from a branch"
3. Select branch: `main`, folder: `/docs`
4. Save

The site will be available at: `https://dayour.github.io/Darbot-Graph-MCP/`

## Customization

### Colors
Edit CSS variables in `assets/css/style.css`:
```css
:root {
    --primary-gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    --cyber-blue: #00d4ff;
    --cyber-purple: #9d4edd;
    /* ... */
}
```

### Clippy Responses
Edit knowledge base in `assets/js/clippy.js`:
```javascript
const knowledgeBase = {
    'setup': 'Your custom response...',
    // ...
};
```

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers

## License

MIT License - See main repository for details.
