# GitHub Pages Deployment Instructions

## Quick Setup

To enable the GitHub Pages site for Darbot Graph MCP:

### Step 1: Enable GitHub Pages
1. Go to the repository on GitHub: https://github.com/dayour/Darbot-Graph-MCP
2. Click on **Settings** tab
3. In the left sidebar, click **Pages**
4. Under "Build and deployment":
   - **Source**: Select "Deploy from a branch"
   - **Branch**: Select `main`
   - **Folder**: Select `/docs`
5. Click **Save**

### Step 2: Wait for Deployment
- GitHub will automatically build and deploy your site
- This usually takes 1-2 minutes
- You'll see a green checkmark when it's ready

### Step 3: Access Your Site
Your site will be available at:
```
https://dayour.github.io/Darbot-Graph-MCP/
```

## Customization

### Update Site URL in Files
If your repository name is different, update the `baseurl` in `docs/_config.yml`:
```yaml
baseurl: "/Your-Repo-Name"
```

### Add Custom Domain (Optional)
1. In repository Settings → Pages
2. Under "Custom domain", enter your domain
3. Add a CNAME record in your DNS settings pointing to: `dayour.github.io`

## Site Features

The deployed site includes:
- **Homepage**: Retro Cyber Modern design with gradient backgrounds
- **Documentation**: Complete setup and configuration guides
- **API Reference**: All 64+ tools with examples
- **Examples**: Real-world usage scenarios
- **Extensibility**: Guide for adding custom tools
- **Wiki**: FAQ, glossary, and resources
- **Clippy Assistant**: Interactive AI-powered help widget

## Troubleshooting

### Site Not Loading
- Check that GitHub Pages is enabled in Settings
- Verify the branch is set to `main` and folder to `/docs`
- Wait a few minutes for initial deployment

### 404 Errors
- Ensure all file paths are relative
- Check that `_config.yml` has correct baseurl

### CSS/JS Not Loading
- Clear browser cache
- Check browser console for errors
- Verify all asset paths are correct

## Local Testing

To test the site locally before deploying:

```bash
cd docs
python -m http.server 8000
# Visit http://localhost:8000
```

Or use any static file server:
```bash
npx serve docs
```

## Updates

To update the site:
1. Make changes to files in the `docs/` directory
2. Commit and push to the `main` branch
3. GitHub Pages will automatically rebuild and deploy

Changes typically appear within 1-2 minutes.

## Support

For issues with the GitHub Pages site:
- Check the [GitHub Pages documentation](https://docs.github.com/en/pages)
- Review deployment logs in the Actions tab
- Open an issue in the repository

---

**Ready to go!** The site is production-ready and waiting for deployment. 🚀
