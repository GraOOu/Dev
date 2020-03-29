
def permutate ( elements, sequence ):
    """
    :param elements: elements of the set to permutate
    :type elements: list
    :param sequence: sequence 
    :type sequence: list

    Recursivly create all possibles permutations from elements.
    if n is elements count it will generate n! results.
    """
    if not elements:
        print ( sequence )
        #return sequence
    
    results = []
    for i in elements:
        nextElements = elements.copy ( )
        nextElements.remove ( i )
        # a moving index can avoid this copy
        nextSequence = sequence.copy ( )
        nextSequence.append ( i )
        #results.append  ( permutate ( nextElements, nextSequence ) )
        permutate ( nextElements, nextSequence )

    return results

sequence  = [ ]
elements = [ 0, 1, 2, 3, 4, 5, 6, 7, 8 ]
results = permutate ( elements, sequence )