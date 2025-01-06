"use client";

import { Flex, Heading, Image, Box, Text } from "@chakra-ui/react";
import logoImage from "../../../../public/logo.png";
import { swrKeys } from "@/typings/swrKeys";
import useSWR from "swr";
import { useRouter } from "next/navigation";

interface IAuthHeaderProps {
  canLogout: boolean;
  title?: string;
}

export const AuthHeader = ({ canLogout, title }: IAuthHeaderProps) => {
  const { mutate } = useSWR(swrKeys.me);
  const router = useRouter();
  const logOut = async () => {
    localStorage.setItem("loginInfo", "");
    await mutate(null);
    router.push("/");
  };

  return (
    <Box height="15%" width="100%">
      <Flex
        direction="row"
        bg="orange.400"
        height="100%"
        alignItems="center"
        justifyContent="space-between"
        paddingX="40px"
      >
        <Flex
          direction="row"
          justifyContent="space-between"
          height="100%"
          alignItems="center"
          width="60%"
        >
          <Image
            src={logoImage.src}
            alt="Naslovna slika showa"
            objectFit="cover"
            height={"80%"}
          />
          {title && (
            <Heading color="white" fontSize="xx-large">
              {title}
            </Heading>
          )}
          {!title && (
            <Heading color="white" fontSize="xx-large">
              eZgrada
            </Heading>
          )}
        </Flex>
        {canLogout && (
          <Text color="white" onClick={logOut} cursor="pointer">
            Log out
          </Text>
        )}
      </Flex>
    </Box>
  );
};