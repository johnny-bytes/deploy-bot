import { IButtonStyles, IPivotStyles, mergeStyleSets } from '@fluentui/react'

export const mainStyles = mergeStyleSets({
  logo: { height: '5rem' },
  navigation: { backgroundColor: '#333' },
})

export const pivotStyle: Partial<IPivotStyles> = {
  linkIsSelected: {
    selectors: {
      '&:hover': {
        backgroundColor: 'rgba(0, 0, 0, .15)',
      },
      '&:active': {
        backgroundColor: 'rgba(0, 0, 0, .25)',
      },
    },
  },
  link: {
    selectors: {
      '&:hover': {
        backgroundColor: 'rgba(0, 0, 0, .15)',
      },
      '&:active': {
        backgroundColor: 'rgba(0, 0, 0, .25)',
      },
    },
  },
}

export const buttonStyle: Partial<IButtonStyles> = {
  rootHovered: {
    backgroundColor: 'rgba(0, 0, 0, .15)',
  },
  rootPressed: {
    backgroundColor: 'rgba(0, 0, 0, .25)',
  },
}
