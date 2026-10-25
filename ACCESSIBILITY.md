# Accessibility Statement (A11y)

The **Sharphy** project is committed to providing an inclusive, barrier-free developer experience for software engineers, security researchers, and automated AI agents of all abilities.

---

## 1. Conformance Standards

Our documentation portal, CLI terminal interfaces, and project guidelines strive to adhere to the **Web Content Accessibility Guidelines (WCAG) 2.1 Level AA** standards:

1. **Color Contrast & Readability:**
   - Documentation color tokens in both dark and light modes maintain minimum contrast ratios of 4.5:1 for standard body copy and 3:1 for large typography and active UI controls.
2. **Keyboard Navigation:**
   - All interactive documentation elements, navigation bars, code copying snippets, and theme selectors are fully operable via keyboard navigation (`Tab`, `Shift+Tab`, `Enter`, `Escape`).
3. **Screen Reader Compatibility:**
   - Documentation uses semantic HTML landmarks (`<main>`, `<header>`, `<footer>`, `<nav>`, `<article>`), explicit heading hierarchies (`h1` through `h4`), and descriptive `aria-label` attributes on interactive icon buttons.
4. **Terminal CLI Readability:**
   - CLI output streams are formatted for standard monospaced terminals, supporting high-contrast mode, avoiding reliance on color alone to convey error states, and directing diagnostic errors strictly to `stderr`.

---

## 2. Feedback & Reporting Accessibility Barriers

We welcome feedback on the accessibility of Sharphy. If you encounter accessibility barriers, low-contrast text, or assistive technology incompatibilities:

- **GitHub Issues:** Open an issue titled `[A11Y]: <description>` on [GitHub Issues](https://github.com/baharmaulidiii/sharphy/issues).
- Please specify the assistive tool (e.g., NVDA, JAWS, VoiceOver), terminal emulator, or browser used.
