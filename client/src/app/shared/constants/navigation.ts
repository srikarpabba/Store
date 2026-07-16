export interface NavItem {
    label: string;
    path: string;
    highlight?: boolean;
}

export const NAV_ITEMS: readonly NavItem[] = [
    { label: 'New', path: '/new' },
    { label: 'Women', path: '/women' },
    { label: 'Men', path: '/men' },
    { label: 'Sale', path: '/sale', highlight: true },
];